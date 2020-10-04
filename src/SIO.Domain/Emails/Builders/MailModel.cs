using System.Collections.Generic;
using OpenEventSourcing.Events;
using SIO.Domain.Emails.Serialization;
using SIO.Domain.Users.Projections;

namespace SIO.Domain.Emails.Builders
{
    public class MailModel<TEvent> 
        where TEvent : IEvent
    {
        public TEvent Event { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
        public User User { get; set; }

        public MailModel()
        {

        }

        public MailModel(Payload<TEvent> payload, User user)
        {
            Event = payload.Event;
            Metadata = payload.Metadata;
            User = user;
        }
    }
}
