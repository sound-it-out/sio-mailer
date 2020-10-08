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
using SIO.Testing.Attributes;
using SIO.Testing.Fakes.Events;
using SIO.Testing.Fakes.Notifications.Builders;
using SIO.Testing.Specifications;

namespace SIO.Domain.Tests.Emails.CommandHandlers.ProcessEmailCommandHandler.ExecuteAsync
{
    public class WhenEmailIsNotSuccessful : CommandHandlerSpecification<ProcessEmailCommand>
    {
        private readonly Guid _aggregateId = Guid.NewGuid().ToSequentialGuid();
        private readonly Guid _correlationId = Guid.NewGuid().ToSequentialGuid();
        private readonly string _userId = Guid.NewGuid().ToSequentialGuid().ToString();
        private readonly Guid _recipientId = Guid.NewGuid().ToSequentialGuid();
        private readonly string _subject = "Test subject";
        private readonly string _template = "Test template";
        private readonly string _payload = "Test payload";
        private readonly string _type = "Test type";
        private readonly string _exception = "Test exception";

        protected override void BuildServices(IServiceCollection services)
        {
            base.BuildServices(services);

            services.RemoveAll<IMailMessageBuilder>();
            services.RemoveAll<IEmailProcessor>();

            services.AddSingleton<IMailMessageBuilder, FakeMailMessageBuilder>();
            services.AddSingleton<IEmailProcessor>(new FakeEmailProcessor(true, _exception));
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
        public async Task EmailFailedEventShouldBeCreated()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();

            using (var context = contextFactory.Create())
            {
                var @event = await context.Events.Where(e => e.AggregateId == _aggregateId).LastAsync();
                @event.Type = typeof(EmailFailed).FullName;
            }
        }

        [Then]
        public async Task EmailFailedEventShouldBeCreatedWithCorrectAggregateId()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).LastAsync();
                var @event = eventDeserializer.Deserialize<EmailFailed>(eventEntity.Data);
                @event.AggregateId.Should().Be(_aggregateId);
            }
        }

        [Then]
        public async Task EmailFailedEventShouldBeCreatedWithCorrectCorrelationId()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).LastAsync();
                var @event = eventDeserializer.Deserialize<EmailFailed>(eventEntity.Data);
                @event.CorrelationId.Should().Be(_correlationId);
            }
        }

        [Then]
        public async Task EmailFailedEventShouldBeCreatedWithCorrectUserId()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).LastAsync();
                var @event = eventDeserializer.Deserialize<EmailFailed>(eventEntity.Data);
                @event.UserId.Should().Be(_userId);
            }
        }

        [Then]
        public async Task EmailFailedEventShouldBeCreatedWithCorrectError()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).LastAsync();
                var @event = eventDeserializer.Deserialize<EmailFailed>(eventEntity.Data);
                @event.Error.Should().Be(_exception);
            }
        }

        [Then]
        public async Task EmailFailedEventShouldBeCreatedWithCorrectVersion()
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory>();
            var eventDeserializer = _serviceProvider.GetRequiredService<IEventDeserializer>();

            using (var context = contextFactory.Create())
            {
                var eventEntity = await context.Events.Where(e => e.AggregateId == _aggregateId).LastAsync();
                var test = await context.Events.ToArrayAsync();
                var @event = eventDeserializer.Deserialize<EmailFailed>(eventEntity.Data);
                @event.Version.Should().Be(2);
            }
        }

        [Then]
        public void EmailFailedEventShouldBePublished()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            eventPublisher.Events.First().Should().BeOfType<EmailFailed>();
        }

        [Then]
        public void EmailFailedEventShouldBePublishedWithCorrectAggregateId()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailFailed)eventPublisher.Events.First();
            @event.AggregateId.Should().Be(_aggregateId);
        }

        [Then]
        public void EmailFailedEventShouldBePublishedWithCorrectCorrelationId()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailFailed)eventPublisher.Events.First();
            @event.CorrelationId.Should().Be(_correlationId);
        }

        [Then]
        public void EmailFailedEventShouldBePublishedWithCorrectUserId()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailFailed)eventPublisher.Events.First();
            @event.UserId.Should().Be(_userId);
        }

        [Then]
        public void EmailFailedEventShouldBePublishedWithCorrectError()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailFailed)eventPublisher.Events.First();
            @event.Error.Should().Be(_exception);
        }

        [Then]
        public void EmailFailedEventShouldBePublishedWithCorrectVersion()
        {
            var eventPublisher = (FakeEventBusPublisher)_serviceProvider.GetRequiredService<IEventBusPublisher>();
            var @event = (EmailFailed)eventPublisher.Events.First();
            @event.Version.Should().Be(2);
        }
    }
}
