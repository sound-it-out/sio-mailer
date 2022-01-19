using SIO.Infrastructure.Events;

namespace SIO.Domain.Emails.Serialization
{
    public interface IPayloadDeserializer
    {
        IEvent Deserialize(string payload);
    }
}
