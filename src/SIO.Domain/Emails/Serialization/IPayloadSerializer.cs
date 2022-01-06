using SIO.Infrastructure.Events;

namespace SIO.Domain.Emails.Serialization
{
    public interface IPayloadSerializer
    {
        string Serialize(IEventContext<IEvent> payload);
    }
}
