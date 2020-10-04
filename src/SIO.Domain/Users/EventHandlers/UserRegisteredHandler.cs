using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Events;
using OpenEventSourcing.Serialization;
using SIO.Domain.Emails;
using SIO.Domain.Emails.Commands;
using SIO.Domain.Emails.Serialization;
using SIO.Domain.Users.Events;

namespace SIO.Domain.Users.EventHandlers
{
    internal sealed class UserRegisteredHandler : IEventHandler<UserRegistered>
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IPayloadSerializer _payloadSerializer;
        private readonly EmailOptions _emailOptions;

        public UserRegisteredHandler(ICommandDispatcher commandDispatcher, 
            IPayloadSerializer payloadSerializer,
            IOptions<EmailOptions> emailOptions)
        {
            if (commandDispatcher == null)
                throw new ArgumentNullException(nameof(commandDispatcher));
            if (payloadSerializer == null)
                throw new ArgumentNullException(nameof(payloadSerializer));
            if (emailOptions == null)
                throw new ArgumentNullException(nameof(emailOptions));

            _commandDispatcher = commandDispatcher;
            _payloadSerializer = payloadSerializer;
            _emailOptions = emailOptions.Value;
        }

        public async Task HandleAsync(UserRegistered @event)
        {
            await _commandDispatcher.DispatchAsync(new QueueEmailCommand(aggregateId: Guid.NewGuid(),
                correlationId: @event.Id,
                userId: @event.UserId,
                recipientId: @event.AggregateId,
                subject: "You have been successfully registered, please activate your account",
                payload: _payloadSerializer.Serialize(@event, new System.Collections.Generic.Dictionary<string, object> { { "ActivateUrl", _emailOptions.Urls.Activate } }),
                template: $"~/Emails/Templates/{nameof(UserRegistered)}/{nameof(UserRegistered)}Email.cshtml",
                type: typeof(UserRegistered).FullName
            ));
        }
    }
}
