using System;
using OpenEventSourcing.Events;

namespace SIO.Domain.Emails.Events
{
    public class EmailSucceded : Event
    {
        public EmailSucceded(Guid aggregateId, int version) : base(aggregateId, version)
        {
        }
    }
}
