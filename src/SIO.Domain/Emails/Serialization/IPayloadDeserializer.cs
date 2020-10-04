using OpenEventSourcing.Events;

namespace SIO.Domain.Emails.Serialization
{
    public interface IPayloadDeserializer
    {
        Payload<TEvent> Deserialize<TEvent>(string payload) where TEvent: IEvent;
    }
}
