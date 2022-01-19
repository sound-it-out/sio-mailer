using SIO.Infrastructure.Events;

namespace SIO.Domain.Emails.Events
{
    public class EmailSucceded : Event
    {
        public string Body { get; }
        public string[] Recipients { get; }
        public string EmailSubject { get; }

        public EmailSucceded(string subject, int version, string body, string[] recipients, string emailSubject) : base(subject, version)
        {
            Body = body;
            Recipients = recipients;
            EmailSubject = emailSubject;
        }
    }
}
