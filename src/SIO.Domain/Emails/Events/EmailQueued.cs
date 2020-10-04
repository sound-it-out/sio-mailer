using System;
using OpenEventSourcing.Events;

namespace SIO.Domain.Emails.Events
{
    public class EmailQueued : Event
    {
        public Guid RecipientId { get; set; }
        public string Subject { get; set; }
        public string Template { get; set; }
        public string Payload { get; set; }
        public string Type { get; set; }
        public EmailQueued(Guid aggregateId, int version, Guid recipientId, string subject, string template, string payload, string type) : base(aggregateId, version)
        {
            RecipientId = recipientId;
            Subject = subject;
            Template = template;
            Payload = payload;
            Type = type;
        }
    }
}
