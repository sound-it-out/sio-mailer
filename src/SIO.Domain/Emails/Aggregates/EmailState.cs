using SIO.Infrastructure.Domain;

namespace SIO.Domain.Emails.Aggregates
{
    public sealed class EmailState : IAggregateState
    {
        public int Attempts { get; set; }
        public EmailStatus Status { get; set; }
        public DateTimeOffset? PublicationDate { get; set; }
        public string Payload { get; set; }
        public string? Body { get; set; }
        public string[]? Recipients { get; set; }
        public string? EmailSubject { get; set; }

        public EmailState() { }
        public EmailState(EmailState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            Attempts = state.Attempts;
            Status = state.Status;
            PublicationDate = state.PublicationDate;
            Payload = state.Payload;
            Body = state.Body;
            Recipients = state.Recipients;
            EmailSubject = state.EmailSubject;
        }
    }
}
