using OpenEventSourcing.Events;
using SIO.Domain.Users.Projections;

namespace SIO.Domain.Emails.Builders
{
    public class MailModel<TEvent> 
        where TEvent : IEvent
    {
        public TEvent Event { get; set; }
        public User User { get; set; }

        public MailModel()
        {

        }

        public MailModel(TEvent @event, User user)
        {
            Event = @event;
            User = user;
        }
    }
}
