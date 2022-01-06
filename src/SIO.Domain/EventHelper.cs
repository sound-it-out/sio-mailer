using SIO.Domain.Emails.Events;

namespace SIO.Domain
{
    public static class EventHelper
    {
        public static Type[] AllEvents = new IntegrationEvents.AllEvents().Concat(new Type[]
        {
            typeof(EmailQueued),
            typeof(EmailFailed),
            typeof(EmailSucceded)
        }).ToArray();

        public static Type[] EmailEvents = new Type[]
        {
            typeof(IntegrationEvents.Users.UserRegistered)
        };
    }
}
