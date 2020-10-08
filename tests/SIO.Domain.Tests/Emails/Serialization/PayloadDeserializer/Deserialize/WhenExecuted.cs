using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using SIO.Domain.Emails.Serialization;
using SIO.Domain.Extensions;
using SIO.Testing.Attributes;
using SIO.Testing.Fakes.Events;
using SIO.Testing.Specifications;

namespace SIO.Domain.Tests.Emails.Serialization.PayloadDeserializer.Deserialize
{
    public class WhenExecuted : Specification<Payload<FakeEvent>>
    {
        private readonly FakeEvent _event = new FakeEvent(Guid.NewGuid(), 1);
        private readonly Dictionary<string, object> _metaData = new Dictionary<string, object> { { "test", "test" } };
        private string EventTimeStamp => JsonConvert.SerializeObject(_event.Timestamp).Replace("\"", "\\\"");
        private string Payload => $"{{\"EventData\":\"{{\\\"id\\\":\\\"{_event.Id}\\\",\\\"aggregateId\\\":\\\"{_event.AggregateId}\\\",\\\"correlationId\\\":null,\\\"causationId\\\":null,\\\"timestamp\\\":{EventTimeStamp},\\\"version\\\":{_event.Version},\\\"userId\\\":null}}\",\"Metadata\":{{\"{_metaData.First().Key}\":\"{_metaData.First().Value}\"}}}}";
        protected override Task<Payload<FakeEvent>> Given()
        {
            var payloadDeserializer = _serviceProvider.GetRequiredService<IPayloadDeserializer>();
            return Task.FromResult(payloadDeserializer.Deserialize<FakeEvent>(Payload));
        }

        protected override Task When()
        {
            return Task.CompletedTask;
        }

        protected override void BuildServices(IServiceCollection services)
        {
            base.BuildServices(services);

            services.AddOpenEventSourcing()
                .AddEvents()
                .AddJsonSerializers();

            services.AddDomain();
        }

        [Then]
        public void ResultShouldHaveEventWithCorrectAggregateId()
        {
            Result.Event.AggregateId.Should().Be(_event.AggregateId);
        }

        [Then]
        public void ResultShouldHaveEventWithCorrectCausationId()
        {
            Result.Event.CausationId.Should().Be(_event.CausationId);
        }

        [Then]
        public void ResultShouldHaveEventWithCorrectCorrelationId()
        {
            Result.Event.CorrelationId.Should().Be(_event.CorrelationId);
        }

        [Then]
        public void ResultShouldHaveEventWithCorrectId()
        {
            Result.Event.Id.Should().Be(_event.Id);
        }

        [Then]
        public void ResultShouldHaveEventWithCorrectTimestamp()
        {
            Result.Event.Timestamp.Should().Be(_event.Timestamp);
        }

        [Then]
        public void ResultShouldHaveEventWithCorrectUserId()
        {
            Result.Event.UserId.Should().Be(_event.UserId);
        }

        [Then]
        public void ResultShouldHaveEventWithCorrectVersion()
        {
            Result.Event.Version.Should().Be(_event.Version);
        }

        [Then]
        public void ResultShouldHaveMetaDataWithCorrectValues()
        {
            Result.Metadata.Should().BeEquivalentTo(_metaData);
        }
    }
}
