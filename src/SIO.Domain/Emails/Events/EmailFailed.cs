using SIO.Infrastructure.Events;

namespace SIO.Domain.Emails.Events
{
    public class EmailFailed : Event
    {
        public string? Body { get; }
        public string[]? Recipients { get; }
        public string Error { get; }

        public EmailFailed(string error, string subject, int version, string? body, string[]? recipients) : base(subject, version)
        {
            Body = body;
            Recipients = recipients;
            Error = error;
        }
    }
}
