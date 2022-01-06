using SIO.Infrastructure.Projections;

namespace SIO.Domain.Emails.Projections
{
    public class EmailFailure : IProjection
    {
        public string Id { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string[]? Recipients { get; set; }
        public string? Error { get; set; }
    }
}
