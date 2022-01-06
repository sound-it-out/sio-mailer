using Microsoft.Extensions.Logging;
using SIO.Domain.Emails.Aggregates;
using SIO.Domain.Emails.Commands;
using SIO.Domain.Emails.Serialization;
using SIO.Infrastructure.Commands;
using SIO.Infrastructure.Domain;
using SIO.Infrastructure.Events;

namespace SIO.Domain.Emails.CommandHandlers
{
    internal sealed class QueueEmailCommandHandler : ICommandHandler<QueueEmailCommand>
    {
        private readonly ILogger<QueueEmailCommandHandler> _logger;
        private readonly IAggregateRepository _aggregateRepository;
        private readonly IAggregateFactory _aggregateFactory;
        private readonly IPayloadSerializer _payloadSerializer;

        public QueueEmailCommandHandler(ILogger<QueueEmailCommandHandler> logger,
            IAggregateRepository aggregateRepository,
            IAggregateFactory aggregateFactory,
            IPayloadSerializer payloadSerializer)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (aggregateRepository == null)
                throw new ArgumentNullException(nameof(aggregateRepository));
            if (aggregateFactory == null)
                throw new ArgumentNullException(nameof(aggregateFactory));

            _logger = logger;
            _aggregateRepository = aggregateRepository;
            _aggregateFactory = aggregateFactory;
            _payloadSerializer = payloadSerializer;
        }

        public async Task ExecuteAsync(QueueEmailCommand command, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(QueueEmailCommandHandler)}.{nameof(ExecuteAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            var aggregate = await _aggregateRepository.GetAsync<Email, EmailState>(command.Subject, cancellationToken);

            if (aggregate != null)
                return;

            aggregate = _aggregateFactory.FromHistory<Email, EmailState>(Enumerable.Empty<IEvent>());

            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            aggregate.Queue(
                subject: command.Subject,
                publicationDate: command.PublicationDate,
                payload: _payloadSerializer.Serialize(command.Event)
            );

            await _aggregateRepository.SaveAsync(aggregate, command, cancellationToken: cancellationToken);
        }
    }
}
