using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Events;
using OpenEventSourcing.Queries;
using OpenEventSourcing.Serialization;
using SIO.Domain.Emails.Aggregates;
using SIO.Domain.Emails.Processors;
using SIO.Domain.Users.Queries;

namespace SIO.Domain.Emails.Builders
{
    internal sealed class MailMessageBuilder : IMailMessageBuilder
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly SmtpOptions _smtpOptions;
        private readonly IRazorViewBuilder _razorViewBuilder;
        private readonly IEventTypeCache _eventTypeCache;
        private readonly IEventDeserializer _eventDeserializer;

        public MailMessageBuilder(IQueryDispatcher queryDispatcher,
            IOptions<SmtpOptions> smtpOptions,
            IRazorViewBuilder razorViewBuilder,
            IEventTypeCache eventTypeCache,
            IEventDeserializer eventDeserializer)
        {
            if (queryDispatcher == null)
                throw new ArgumentNullException(nameof(queryDispatcher));
            if (smtpOptions == null)
                throw new ArgumentNullException(nameof(smtpOptions));
            if (razorViewBuilder == null)
                throw new ArgumentNullException(nameof(razorViewBuilder));
            if (eventTypeCache == null)
                throw new ArgumentNullException(nameof(eventTypeCache));
            if (eventDeserializer == null)
                throw new ArgumentNullException(nameof(eventDeserializer));

            _queryDispatcher = queryDispatcher;
            _smtpOptions = smtpOptions.Value;
            _razorViewBuilder = razorViewBuilder;
            _eventTypeCache = eventTypeCache;
            _eventDeserializer = eventDeserializer;
        }

        public async Task<MailMessage> BuildAsync(EmailState email)
        {
            var userQueryResult = await _queryDispatcher.DispatchAsync(new GetUserByIdQuery(Guid.NewGuid(), Guid.Empty.ToString(), email.RecipientId));

            if(_eventTypeCache.TryGet(email.Template, out var eventType))
            {
                var @event = _eventDeserializer.Deserialize(email.Payload, eventType);
                var modelType = typeof(MailModel<>).MakeGenericType(eventType);
                var model = Activator.CreateInstance(modelType, @event, userQueryResult.User);

                var body = await _razorViewBuilder.BuildAsync(email.Template, model);

                return new MailMessage(
                    from: _smtpOptions.FromAddress,
                    to: userQueryResult.User.Email,
                    subject: email.Subject,
                    body: body
                );
            }
            else
            {
                throw new InvalidOperationException($"Unsupported event type: {email.Template}");
            }
        }
    }
}
