using System;

namespace SIO.Domain.Emails.Projections
{
    public class EmailFailure
    {
        public Guid Id { get; set; }
        public Guid EmailId { get; set; }
        public string Error { get; set; }
    }
}
