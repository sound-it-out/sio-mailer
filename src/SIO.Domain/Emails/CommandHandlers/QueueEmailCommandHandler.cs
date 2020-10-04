using System;
using System.Linq;
using System.Threading.Tasks;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Domain;
using OpenEventSourcing.Events;
using SIO.Domain.Emails.Aggregates;
using SIO.Domain.Emails.Commands;

namespace SIO.Domain.Emails.CommandHandlers
{
    internal class QueueEmailCommandHandler : ICommandHandler<QueueEmailCommand>
    {
        private readonly IAggregateRepository _aggregateRepository;
        private readonly IAggregateFactory _aggregateFactory;
        private readonly IEventBusPublisher _eventBusPublisher;

        public QueueEmailCommandHandler(IAggregateRepository aggregateRepository,
            IAggregateFactory aggregateFactory,
            IEventBusPublisher eventBusPublisher)
        {
            if (aggregateRepository == null)
                throw new ArgumentNullException(nameof(aggregateRepository));
            if (aggregateFactory == null)
                throw new ArgumentNullException(nameof(aggregateFactory));
            if (eventBusPublisher == null)
                throw new ArgumentNullException(nameof(eventBusPublisher));

            _aggregateRepository = aggregateRepository;
            _aggregateFactory = aggregateFactory;
            _eventBusPublisher = eventBusPublisher;
        }

        public async Task ExecuteAsync(QueueEmailCommand command)
        {
            var aggregate = _aggregateFactory.FromHistory<Email, EmailState>(Enumerable.Empty<IEvent>());

            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            aggregate.Queue(aggregateId: command.AggregateId,
                recipientId: command.RecipientId,
                subject: command.Subject,
                payload: command.Payload,
                template: command.Template,
                type: command.Type);

            var events = aggregate.GetUncommittedEvents();

            foreach (var @event in events)
                @event.UpdateFrom(command);

            events = events.ToList();

            await _aggregateRepository.SaveAsync(aggregate, command.Version);
            await _eventBusPublisher.PublishAsync(events);
        }
    }
}
