using System.Collections.Generic;
using OpenEventSourcing.Events;

namespace SIO.Domain.Emails.Serialization
{
    public class Payload<TEvent> 
        where TEvent : IEvent
    {
        public TEvent Event { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    public class Payload
    {
        public string EventData { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
}
