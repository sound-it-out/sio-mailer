using System;
using OpenEventSourcing.Events;

namespace SIO.Domain.Emails.Events
{
    public class EmailFailed : Event
    {
        public string Error { get; set; }

        public EmailFailed(Guid aggregateId, int version, string error) : base(aggregateId, version)
        {
            Error = error;
        }
    }
}
