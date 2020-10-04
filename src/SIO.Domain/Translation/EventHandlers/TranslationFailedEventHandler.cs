using System;
using System.Threading.Tasks;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Events;
using OpenEventSourcing.Serialization;
using SIO.Domain.Emails.Commands;
using SIO.Domain.Translation.Events;

namespace SIO.Domain.Translation.EventHandlers
{
    public class TranslationFailedEventHandler : IEventHandler<TranslationFailed>
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IEventSerializer _eventSerializer;

        public TranslationFailedEventHandler(ICommandDispatcher commandDispatcher, IEventSerializer eventSerializer)
        {
            if (commandDispatcher == null)
                throw new ArgumentNullException(nameof(commandDispatcher));
            if (eventSerializer == null)
                throw new ArgumentNullException(nameof(eventSerializer));

            _commandDispatcher = commandDispatcher;
            _eventSerializer = eventSerializer;
        }

        public async Task HandleAsync(TranslationFailed @event)
        {
            var payload = _eventSerializer.Serialize(@event);
            var template = nameof(TranslationFailed);

            await Task.WhenAll(
                _commandDispatcher.DispatchAsync(new QueueEmailCommand(aggregateId: Guid.NewGuid(),
                    correlationId: @event.Id,
                    userId: @event.UserId,
                    payload: payload,
                    template: template
                )),
                _commandDispatcher.DispatchAsync(new QueueIosNotificationCommand(aggregateId: Guid.NewGuid(),
                    correlationId: @event.Id,
                    userId: @event.UserId,
                    payload: payload,
                    template: template
                )),
                _commandDispatcher.DispatchAsync(new QueueWindowsNotificationCommand(aggregateId: Guid.NewGuid(),
                    correlationId: @event.Id,
                    userId: @event.UserId,
                    payload: payload,
                    template: template
                ))
            );
        }
    }
}
