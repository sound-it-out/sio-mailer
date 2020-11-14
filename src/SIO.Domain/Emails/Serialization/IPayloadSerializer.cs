using System.Collections.Generic;
using OpenEventSourcing.Events;

namespace SIO.Domain.Emails.Serialization
{
    public interface IPayloadSerializer
    {
        string Serialize<TEvent>(TEvent @event, Dictionary<string, object> metadata = null) where TEvent : IEvent;
    }
}
