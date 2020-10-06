using System;

namespace SIO.Domain.Emails.Projections
{
    public class EmailQueue
    {
        public Guid Id { get; set; }
        public int Attempts { get; set; }
        public EmailStatus Status { get; set; }
        public string Subject { get; set; }
        public string Template { get; set; }
        public string Payload { get; set; }
        public string Type { get; set; }
        public Guid RecipientId { get; set; }
        public int Version { get; set; }
    }
}
