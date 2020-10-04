using System;
using System.Collections.Generic;
using OpenEventSourcing.Domain;

namespace SIO.Domain.Emails.Aggregates
{
    public class EmailState : IAggregateState
    {
        public int Attempts { get; set; }
        public string Subject { get; set; }
        public string Payload { get; set; }
        public string Template { get; set; }
        public Guid RecipientId { get; set; }
        public EmailStatus Status { get; set; }

        public EmailState()
        {
        }

        public EmailState(EmailState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            Attempts = state.Attempts;
            Subject = state.Subject;
            Payload = state.Payload;
            Template = state.Template;
            RecipientId = state.RecipientId;
            Status = state.Status;
        }
    }
}
