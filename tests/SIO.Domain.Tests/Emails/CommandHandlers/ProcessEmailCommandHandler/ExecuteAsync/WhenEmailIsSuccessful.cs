using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization;
using SIO.Domain.Emails.Builders;
using SIO.Domain.Emails.Commands;
using SIO.Domain.Emails.Events;
using SIO.Domain.Emails.Processors;
using SIO.Testing.Abstractions;
using SIO.Testing.Attributes;
using SIO.Testing.Fakes.Events;
using SIO.Testing.Fakes.Notifications.Builders;

namespace SIO.Domain.Tests.Emails.CommandHandlers.ProcessEmailCommandHandler.ExecuteAsync
{
    public class WhenEmailIsSuccessful : CommandHandlerSpecification<ProcessEmailCommand>
    {
        private readonly Guid _aggregateId = Guid.NewGuid().ToSequentialGuid();
        private readonly Guid _correlationId = Guid.NewGuid().ToSequentialGuid();
        private readonly string _userId = Guid.NewGuid().ToSequentialGuid().ToString();
        private readonly Guid _recipientId = Guid.NewGuid().ToSequentialGuid();
        private readonly string _subject = "Test subject";
        private readonly string _template = "Test template";
        private readonly string _payload = "Test payload";
        private readonly string _type = "Test type";

        protected override void BuildServices(IServiceCollection services)
        {
            base.BuildServices(services);

            services.RemoveAll<IMailMessageBuilder>();
            services.RemoveAll<IEmailProcessor>();

            services.AddSingleton<IMailMessageBuilder, FakeMailMessageBuilder>();
            services.AddSingleton<IEmailProcessor>(new FakeEmailProcessor(false, ""));
        }

        protected override ProcessEmailCommand Given()
        {
            return new ProcessEmailCommand(_aggregateId, _correlationId, 1, _userId);
        }

        protected override async Task When()
        {
            await _commandDispatcher.DispatchAsync(new QueueEmailCommand(_aggregateId, _correlationId, _userId, _recipientId, _subject, _payload, _template, _type));
        }

        [Then]
        public async Task EmailSuccededEventShouldBeCreated()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();

            using (var context = contextFactory.Create())
            {
                var @event = await context.Events.Where(e => e.AggregateId == _aggregateId).LastAsync();
                @event.Type = typeof(EmailSucceded).FullName;
            }
        }

        [Then]
        public async Task EmailSuccededEventShouldBeCreatedWithCorrectAggregateId()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).LastAsync();
                var @event = eventDeserializer.Deserialize<EmailSucceded>(eventEntity.Data);
                @event.AggregateId.Should().Be(_aggregateId);
            }
        }

        [Then]
        public async Task EmailSuccededEventShouldBeCreatedWithCorrectCorrelationId()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).LastAsync();
                var @event = eventDeserializer.Deserialize<EmailSucceded>(eventEntity.Data);
                @event.CorrelationId.Should().Be(_correlationId);
            }
        }

        [Then]
        public async Task EmailSuccededEventShouldBeCreatedWithCorrectUserId()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).LastAsync();
                var @event = eventDeserializer.Deserialize<EmailSucceded>(eventEntity.Data);
                @event.UserId.Should().Be(_userId);
            }
        }

        [Then]
        public async Task EmailSuccededEventShouldBeCreatedWithCorrectVersion()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).LastAsync();
                var test = await context.Events.ToArrayAsync();
                var @event = eventDeserializer.Deserialize<EmailSucceded>(eventEntity.Data);
                @event.Version.Should().Be(2);
            }
        }

        [Then]
        public void EmailSuccededEventShouldBePublished()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            eventPublisher.Events.First().Should().BeOfType<EmailSucceded>();
        }

        [Then]
        public void EmailSuccededEventShouldBePublishedWithCorrectAggregateId()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailSucceded)eventPublisher.Events.First();
            @event.AggregateId.Should().Be(_aggregateId);
        }

        [Then]
        public void EmailSuccededEventShouldBePublishedWithCorrectCorrelationId()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailSucceded)eventPublisher.Events.First();
            @event.CorrelationId.Should().Be(_correlationId);
        }

        [Then]
        public void EmailSuccededEventShouldBePublishedWithCorrectUserId()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailSucceded)eventPublisher.Events.First();
            @event.UserId.Should().Be(_userId);
        }

        [Then]
        public void EmailSuccededEventShouldBePublishedWithCorrectVersion()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailSucceded)eventPublisher.Events.First();
            @event.Version.Should().Be(2);
        }
    }
}
