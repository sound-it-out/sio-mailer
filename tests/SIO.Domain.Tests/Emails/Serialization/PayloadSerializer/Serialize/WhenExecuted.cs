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

namespace SIO.Domain.Tests.Emails.Serialization.PayloadSerializer.Serialize
{
    public class WhenExecuted : Specification<string>
    {
        private readonly FakeEvent _event = new FakeEvent(Guid.NewGuid(), 1);
        private readonly Dictionary<string, object> _metaData = new Dictionary<string, object> { { "test", "test" } };
        private string EventTimeStamp => JsonConvert.SerializeObject(_event.Timestamp).Replace("\"", "\\\"");
        private string Payload => $"{{\"EventData\":\"{{\\\"id\\\":\\\"{_event.Id}\\\",\\\"aggregateId\\\":\\\"{_event.AggregateId}\\\",\\\"correlationId\\\":null,\\\"causationId\\\":null,\\\"timestamp\\\":{EventTimeStamp},\\\"version\\\":{_event.Version},\\\"userId\\\":null}}\",\"Metadata\":{{\"{_metaData.First().Key}\":\"{_metaData.First().Value}\"}}}}";
        protected override Task<string> Given()
        {
            var payloadSerializer = _serviceProvider.GetRequiredService<IPayloadSerializer>();
            return Task.FromResult(payloadSerializer.Serialize(_event, _metaData));
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
        public void ResultShouldBeSerializedWithCorrectValue()
        {
            Result.Should().Be(Payload);
        }
    }
}
