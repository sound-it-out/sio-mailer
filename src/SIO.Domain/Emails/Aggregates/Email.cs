using System;
using OpenEventSourcing.Domain;
using SIO.Domain.Emails.Events;

namespace SIO.Domain.Emails.Aggregates
{
    public class Email : Aggregate<EmailState>
    {
        public Email(EmailState state) : base(state)
        {
            Handles<EmailQueued>(Handle);
            Handles<EmailFailed>(Handle);
            Handles<EmailSucceded>(Handle);
        }

        public override EmailState GetState() => new EmailState(_state);

        public void Queue(Guid aggregateId, string subject, string payload, string template, string type, Guid recipientId) => Apply(new EmailQueued(aggregateId, Version.GetValueOrDefault() + 1, recipientId, subject, template, payload, type));
        public void MarkAsFail(string error) => Apply(new EmailFailed(Id.Value, Version.GetValueOrDefault() + 1, error));
        public void MarkAsSuccess() => Apply(new EmailSucceded(Id.Value, Version.GetValueOrDefault() + 1));

        public void Handle(EmailQueued @event)
        {
            //if(Id.HasValue)
            // throw exception

            Id = @event.AggregateId;
            _state.Attempts = 0;
            _state.RecipientId = @event.RecipientId;
            _state.Subject = @event.Subject;
            _state.Payload = @event.Payload;
            _state.Template = @event.Template;
            _state.Type = @event.Type;
            _state.Status = EmailStatus.Pending;
            Version = @event.Version;
        }

        public void Handle(EmailFailed @event)
        {
            _state.Attempts++;
            _state.Status = EmailStatus.Failed;
            Version = @event.Version;
        }

        public void Handle(EmailSucceded @event)
        {
            _state.Attempts++;
            _state.Status = EmailStatus.Success;
            Version = @event.Version;
        }
    }
}
