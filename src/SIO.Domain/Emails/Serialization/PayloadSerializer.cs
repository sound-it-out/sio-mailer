using Newtonsoft.Json;
using SIO.Infrastructure.Events;

namespace SIO.Domain.Emails.Serialization
{
    internal sealed class PayloadSerializer : IPayloadSerializer
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public PayloadSerializer()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            };
        }
        
        public string Serialize(IEventContext<IEvent> payload)
        {

            return JsonConvert.SerializeObject(new PayloadContext
            {
                Type = payload.Payload.GetType().FullName,
                Payload = JsonConvert.SerializeObject(payload.Payload, _serializerSettings)
            },
            _serializerSettings);
        }
    }
}
