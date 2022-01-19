using SIO.Domain.Emails.Events;
using SIO.Infrastructure.Domain;
using System;

namespace SIO.Domain.Emails.Aggregates
{
    public sealed class Email : Aggregate<EmailState>
    {
        public Email(EmailState state) : base(state)
        {
            Handles<EmailQueued>(Handle);
            Handles<EmailFailed>(Handle);
            Handles<EmailSucceded>(Handle);
        }

        public override EmailState GetState() => new EmailState(_state);

        public void Queue(string subject,
            DateTimeOffset? publicationDate,
            string payload)
        {
            Apply(new EmailQueued(
                subject: subject,
                version: Version + 1,
                publicationDate: publicationDate,
                payload: payload
            ));
        }

        public void Fail(string error, string? body, string[]? recipients)
        {
            Apply(new EmailFailed(
                error: error,
                subject: Id,
                version: Version + 1,
                body: body,
                recipients: recipients
            ));
        }

        public void Succeed(string body, string[] recipients, string emailSubject)
        {
            Apply(new EmailSucceded(
                subject: Id,
                version: Version + 1,
                body: body,
                recipients: recipients,
                emailSubject: emailSubject
            ));
        }

        private void Handle(EmailQueued @event)
        {
            Id = @event.Subject;
            _state.PublicationDate = @event.PublicationDate;
            _state.Attempts = 0;
            _state.Status = EmailStatus.Queued;
            _state.Payload = @event.Payload;
            Version = @event.Version;
        }

        private void Handle(EmailFailed @event)
        {
            _state.Attempts++;
            _state.Status = EmailStatus.Failed;
            Version = @event.Version;
        }

        private void Handle(EmailSucceded @event)
        {
            _state.Attempts++;
            _state.Status = EmailStatus.Succeeded;
            _state.Body = @event.Body;
            _state.Recipients = @event.Recipients;
            _state.EmailSubject = @event.EmailSubject;
            Version = @event.Version;
        }
    }
}
