using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Events;
using SIO.Domain.Emails;
using SIO.Domain.Emails.Commands;
using SIO.Domain.Emails.Serialization;
using SIO.Domain.Users.Events;

namespace SIO.Domain.Users.EventHandlers
{
    internal sealed class UserPasswordTokenGeneratedHandler : IEventHandler<UserPasswordTokenGenerated>
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IPayloadSerializer _payloadSerializer;
        private readonly EmailOptions _emailOptions;

        public UserPasswordTokenGeneratedHandler(ICommandDispatcher commandDispatcher, 
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

        public async Task HandleAsync(UserPasswordTokenGenerated @event)
        {
            await _commandDispatcher.DispatchAsync(new QueueEmailCommand(aggregateId: Guid.NewGuid(),
                correlationId: @event.Id,
                userId: @event.UserId,
                recipientId: @event.AggregateId,
                subject: "Password reset",
                payload: _payloadSerializer.Serialize(@event, new Dictionary<string, object> { { "ResetPasswordUrl", _emailOptions.Urls.ResetPassword } }),
                template: $"~/Emails/Templates/{nameof(UserPasswordTokenGenerated)}/{nameof(UserPasswordTokenGenerated)}Email.cshtml",
                type: typeof(UserRegistered).FullName
            ));
        }
    }
}
