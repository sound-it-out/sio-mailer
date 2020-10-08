using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization;
using SIO.Domain.Emails.Commands;
using SIO.Domain.Emails.Events;
using SIO.Testing.Attributes;
using SIO.Testing.Fakes.Events;
using SIO.Testing.Specifications;

namespace SIO.Domain.Tests.Emails.CommandHandlers.QueueEmailCommandHandler.ExecuteAsync
{
    public class WhenExecuted : CommandHandlerSpecification<QueueEmailCommand>
    {
        private readonly Guid _aggregateId = Guid.NewGuid().ToSequentialGuid();
        private readonly Guid _correlationId = Guid.NewGuid().ToSequentialGuid();
        private readonly string _userId = Guid.NewGuid().ToSequentialGuid().ToString();
        private readonly Guid _recipientId = Guid.NewGuid().ToSequentialGuid();
        private readonly string _subject = "Test subject";
        private readonly string _template = "Test template";
        private readonly string _payload = "Test payload";
        private readonly string _type = "Test type";

        protected override QueueEmailCommand Given()
        {
            return new QueueEmailCommand(_aggregateId, _correlationId, _userId, _recipientId, _subject, _payload, _template, _type);
        }

        protected override Task When()
        {
            return Task.CompletedTask;
        }

        [Then]
        public async Task EmailQueuedEventShouldBeCreated()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();

            using (var context = contextFactory.Create())
            {
                var @event = await context.Events.Where(e => e.AggregateId == _aggregateId).FirstOrDefaultAsync();
                @event.Type = typeof(EmailQueued).FullName;
            }
        }

        [Then]
        public async Task EmailQueuedEventShouldBeCreatedWithCorrectAggregateId()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).FirstOrDefaultAsync();
                var @event = eventDeserializer.Deserialize<EmailQueued>(eventEntity.Data);
                @event.AggregateId.Should().Be(_aggregateId);
            }
        }

        [Then]
        public async Task EmailQueuedEventShouldBeCreatedWithCorrectCorrelationId()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).FirstOrDefaultAsync();
                var @event = eventDeserializer.Deserialize<EmailQueued>(eventEntity.Data);
                @event.CorrelationId.Should().Be(_correlationId);
            }
        }

        [Then]
        public async Task EmailQueuedEventShouldBeCreatedWithCorrectUserId()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).FirstOrDefaultAsync();
                var @event = eventDeserializer.Deserialize<EmailQueued>(eventEntity.Data);
                @event.UserId.Should().Be(_userId);
            }
        }

        [Then]
        public async Task EmailQueuedEventShouldBeCreatedWithCorrectRecipientId()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).FirstOrDefaultAsync();
                var @event = eventDeserializer.Deserialize<EmailQueued>(eventEntity.Data);
                @event.RecipientId.Should().Be(_recipientId);
            }
        }

        [Then]
        public async Task EmailQueuedEventShouldBeCreatedWithCorrectSubject()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).FirstOrDefaultAsync();
                var @event = eventDeserializer.Deserialize<EmailQueued>(eventEntity.Data);
                @event.Subject.Should().Be(_subject);
            }
        }

        [Then]
        public async Task EmailQueuedEventShouldBeCreatedWithCorrectPayload()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).FirstOrDefaultAsync();
                var @event = eventDeserializer.Deserialize<EmailQueued>(eventEntity.Data);
                @event.Payload.Should().Be(_payload);
            }
        }

        [Then]
        public async Task EmailQueuedEventShouldBeCreatedWithCorrectTemplate()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).FirstOrDefaultAsync();
                var @event = eventDeserializer.Deserialize<EmailQueued>(eventEntity.Data);
                @event.Template.Should().Be(_template);
            }
        }

        [Then]
        public async Task EmailQueuedEventShouldBeCreatedWithCorrectType()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).FirstOrDefaultAsync();
                var @event = eventDeserializer.Deserialize<EmailQueued>(eventEntity.Data);
                @event.Type.Should().Be(_type);
            }
        }

        [Then]
        public async Task EmailQueuedEventShouldBeCreatedWithCorrectVersion()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).FirstOrDefaultAsync();
                var @event = eventDeserializer.Deserialize<EmailQueued>(eventEntity.Data);
                @event.Version.Should().Be(1);
            }
        }

        [Then]
        public void EmailQueuedEventShouldBePublished()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            eventPublisher.Events.First().Should().BeOfType<EmailQueued>();
        }

        [Then]
        public void EmailQueuedEventShouldBePublishedWithCorrectAggregateId()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailQueued)eventPublisher.Events.First();
            @event.AggregateId.Should().Be(_aggregateId);
        }

        [Then]
        public void EmailQueuedEventShouldBePublishedWithCorrectCorrelationId()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailQueued)eventPublisher.Events.First();
            @event.CorrelationId.Should().Be(_correlationId);
        }

        [Then]
        public void EmailQueuedEventShouldBePublishedWithCorrectUserId()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailQueued)eventPublisher.Events.First();
            @event.UserId.Should().Be(_userId);
        }

        [Then]
        public void EmailQueuedEventShouldBePublishedWithCorrectRecipientId()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailQueued)eventPublisher.Events.First();
            @event.RecipientId.Should().Be(_recipientId);
        }

        [Then]
        public void EmailQueuedEventShouldBePublishedWithCorrectSubject()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailQueued)eventPublisher.Events.First();
            @event.Subject.Should().Be(_subject);
        }

        [Then]
        public void EmailQueuedEventShouldBePublishedWithCorrectPayload()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailQueued)eventPublisher.Events.First();
            @event.Payload.Should().Be(_payload);
        }

        [Then]
        public void EmailQueuedEventShouldBePublishedWithCorrectTemplate()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailQueued)eventPublisher.Events.First();
            @event.Template.Should().Be(_template);
        }

        [Then]
        public void EmailQueuedEventShouldBePublishedWithCorrectType()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailQueued)eventPublisher.Events.First();
            @event.Type.Should().Be(_type);
        }

        [Then]
        public void EmailQueuedEventShouldBePublishedWithCorrectVersion()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailQueued)eventPublisher.Events.First();
            @event.Version.Should().Be(1);
        }
    }
}
