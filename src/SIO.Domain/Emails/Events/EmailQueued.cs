using SIO.Infrastructure.Events;
using System;

namespace SIO.Domain.Emails.Events
{
    public class EmailQueued : Event
    {
        public DateTimeOffset? PublicationDate { get; }
        public string Payload { get; }

        public EmailQueued(string subject,
            int version,
            DateTimeOffset? publicationDate,
            string payload) : base(subject, version)
        {
            PublicationDate = publicationDate;
            Payload = payload;
        }
    }
}
