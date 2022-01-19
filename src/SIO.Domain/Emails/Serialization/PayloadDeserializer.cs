using System;
using Newtonsoft.Json;
using SIO.Infrastructure.Events;

namespace SIO.Domain.Emails.Serialization
{
    internal sealed class PayloadDeserializer : IPayloadDeserializer
    {
        private readonly IEventTypeCache _eventTypeCache;
        private readonly JsonSerializerSettings _serializerSettings;

        public PayloadDeserializer(IEventTypeCache eventTypeCache)
        {
            if (eventTypeCache == null)
                throw new ArgumentNullException(nameof(eventTypeCache));

            _eventTypeCache = eventTypeCache;
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            };
        }

        public IEvent Deserialize(string payload)
        {
            var payloadContext = JsonConvert.DeserializeObject<PayloadContext>(payload, _serializerSettings);

            if (_eventTypeCache.TryGet(payloadContext.Type, out var type))
                return (IEvent)JsonConvert.DeserializeObject(payloadContext.Payload, type, _serializerSettings);

            throw new InvalidOperationException($"Payload of type: {payloadContext.Type} is unsupported");
        }
    }
}
