using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OpenEventSourcing.Events;
using OpenEventSourcing.Serialization;

namespace SIO.Domain.Emails.Serialization
{
    internal sealed class PayloadSerializer : IPayloadSerializer
    {
        private readonly IEventSerializer _eventSerializer;
        private readonly JsonSerializerSettings _serializerSettings;

        public PayloadSerializer(IEventSerializer eventSerializer)
        {
            if (eventSerializer == null)
                throw new ArgumentNullException(nameof(eventSerializer));

            _eventSerializer = eventSerializer;
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            };
        }

        public string Serialize<TEvent>(TEvent @event, Dictionary<string, object> metadata) where TEvent : IEvent
        {
            return JsonConvert.SerializeObject(new Payload
            {
                EventData = _eventSerializer.Serialize(@event),
                Metadata = metadata ?? new Dictionary<string, object>()
            },
            _serializerSettings);
        }
    }
}
