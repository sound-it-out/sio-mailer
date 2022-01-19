using SIO.Infrastructure;
using SIO.Infrastructure.Commands;
using SIO.Infrastructure.Events;
using System;

namespace SIO.Domain.Emails.Commands
{
    public class QueueEmailCommand : Command
    {
        public DateTimeOffset? PublicationDate { get; }
        public IEventContext<IEvent> Event { get; }

        public QueueEmailCommand(string subject,
            CorrelationId? correlationId,
            int version,
            Actor actor,
            DateTimeOffset? publicationDate,
            IEventContext<IEvent> @event) : base(subject, correlationId, version, actor)
        {
            PublicationDate = publicationDate;
            Event = @event;
        }
    }
}
