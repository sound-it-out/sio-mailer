using SIO.Infrastructure.Projections;
using System;

namespace SIO.Domain.Emails.Projections
{
    public class EmailQueue : IProjection
    {
        public string Subject { get; set; }
        public int Attempts { get; set; }
        public DateTimeOffset? PublicationDate { get; set; }
        public string Payload { get; set; }
    }
}
