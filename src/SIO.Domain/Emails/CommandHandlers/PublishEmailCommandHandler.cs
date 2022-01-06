using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SIO.Domain.Emails.Aggregates;
using SIO.Domain.Emails.Commands;
using SIO.Domain.Emails.Serialization;
using SIO.Domain.Emails.Templates;
using SIO.Infrastructure.Commands;
using SIO.Infrastructure.Domain;
using SIO.Infrastructure.Events;

namespace SIO.Domain.Emails.CommandHandlers
{
    internal sealed class PublishEmailCommandHandler : ICommandHandler<PublishEmailCommand>
    {
        private readonly ILogger<PublishEmailCommandHandler> _logger;
        private readonly IAggregateRepository _aggregateRepository;
        private readonly IEventStore _eventStore;
        private readonly SmtpOptions _smtpOptions;
        private readonly IMailMessageBuilder _mailMessageBuilder;
        private readonly IPayloadDeserializer _payloadDeserializer;

        public PublishEmailCommandHandler(ILogger<PublishEmailCommandHandler> logger,
            IAggregateRepository aggregateRepository,
            IEventStore eventStore,
            IOptionsSnapshot<SmtpOptions> optionsSnapshot,
            IMailMessageBuilder mailMessageBuilder,
            IPayloadDeserializer payloadDeserializer)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (aggregateRepository == null)
                throw new ArgumentNullException(nameof(aggregateRepository));
            if (eventStore == null)
                throw new ArgumentNullException(nameof(eventStore));
            if (optionsSnapshot == null)
                throw new ArgumentNullException(nameof(optionsSnapshot));
            if (mailMessageBuilder == null)
                throw new ArgumentNullException(nameof(mailMessageBuilder));
            if (payloadDeserializer == null)
                throw new ArgumentNullException(nameof(payloadDeserializer));

            _logger = logger;
            _aggregateRepository = aggregateRepository;            
            _eventStore = eventStore;
            _smtpOptions = optionsSnapshot.Value;
            _mailMessageBuilder = mailMessageBuilder;
            _payloadDeserializer = payloadDeserializer;
        }

        public async Task ExecuteAsync(PublishEmailCommand command, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(PublishEmailCommandHandler)}.{nameof(ExecuteAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            var aggregate = await _aggregateRepository.GetAsync<Email, EmailState>(command.Subject, cancellationToken);

            MailMessage? mailMessage = null;

            try
            {
                var state = aggregate.GetState();
                // Event has been published but we are ahead of projections!
                if (state.Status == EmailStatus.Succeeded)
                    return;

                var payload = _payloadDeserializer.Deserialize(state.Payload);
                mailMessage = await _mailMessageBuilder.BuildAsync(payload);

                using (var smtpClient = new SmtpClient(_smtpOptions.Host, _smtpOptions.Port))
                {
                    smtpClient.DeliveryMethod = _smtpOptions.DeliveryMethod;
                    smtpClient.EnableSsl = _smtpOptions.UseSsl;

                    if (!string.IsNullOrWhiteSpace(_smtpOptions.Username))
                        smtpClient.Credentials = new NetworkCredential(_smtpOptions.Username, _smtpOptions.Password);

                    if (smtpClient.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory)
                        smtpClient.PickupDirectoryLocation = _smtpOptions.DirectoryPickupLocation;

                    await smtpClient.SendMailAsync(mailMessage);
                }             

                aggregate.Succeed(mailMessage.Body, mailMessage.To.Select(t => t.Address).ToArray(), mailMessage.Subject);
            }
            catch (Exception ex)
            {
                aggregate.Fail(ex.Message, mailMessage?.Body, mailMessage?.To.Select(t => t.Address).ToArray());
            }

            await _aggregateRepository.SaveAsync(aggregate, command, aggregate.Version - 1, cancellationToken);
        }        
    }
}
