using System;
using Newtonsoft.Json;
using OpenEventSourcing.Events;

namespace SIO.Domain.Translation.Events
{
    public class TranslationFailed : Event
    {
        public string Error { get; }

        public TranslationFailed(Guid aggregateId, int version, string error) : base(aggregateId, version)
        {
            Error = error;
        }

        [JsonConstructor]
        public TranslationFailed(Guid aggregateId, Guid correlationId, Guid userId, int version, string error) : base(aggregateId, version)
        {
            Error = error;
            CorrelationId = correlationId;
            UserId = userId.ToString();
        }
    }
}
