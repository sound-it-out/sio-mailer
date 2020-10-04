using System;
using OpenEventSourcing.Commands;

namespace SIO.Domain.Emails.Commands
{
    public class QueueEmailCommand : Command
    {
        public Guid RecipientId { get; set; }
        public string Subject { get; set; }
        public string Payload { get; set; }
        public string Template { get; set; }
        public string Type { get; set; }

        public QueueEmailCommand(Guid aggregateId, Guid correlationId, string userId, Guid recipientId, string subject, string payload, string template, string type) : base(aggregateId, correlationId, 0, userId)
        {
            RecipientId = recipientId;
            Subject = subject;
            Payload = payload;
            Template = template;
            Type = type;
        }
    }
}
