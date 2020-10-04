using System;
using Newtonsoft.Json;
using OpenEventSourcing.Events;
using OpenEventSourcing.Serialization;

namespace SIO.Domain.Emails.Serialization
{
    internal sealed class PayloadDeserializer : IPayloadDeserializer
    {
        private readonly IEventDeserializer _eventDeserializer;
        private readonly JsonSerializerSettings _serializerSettings;

        public PayloadDeserializer(IEventDeserializer eventDeserializer)
        {
            if (eventDeserializer == null)
                throw new ArgumentNullException(nameof(eventDeserializer));

            _eventDeserializer = eventDeserializer;
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            };
        }

        public Payload<TEvent> Deserialize<TEvent>(string payload) where TEvent : IEvent
        {
            var payloadModel = JsonConvert.DeserializeObject<Payload>(payload);
            return new Payload<TEvent>
            {
                Event = _eventDeserializer.Deserialize<TEvent>(payloadModel.EventData),
                Metadata = payloadModel.Metadata
            };
        }
    }
}
