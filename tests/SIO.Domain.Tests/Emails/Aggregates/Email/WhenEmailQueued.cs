using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using SIO.Domain.Emails.Aggregates;
using SIO.Domain.Emails.Events;
using SIO.Testing.Abstractions;
using SIO.Testing.Attributes;

namespace SIO.Domain.Tests.Emails.Aggregates.Email
{
    public class WhenEmailQueued : AggregateSpecification<Domain.Emails.Aggregates.Email, EmailState>
    {
        private readonly Guid _aggregateId = Guid.NewGuid().ToSequentialGuid();
        private readonly Guid _recipientId = Guid.NewGuid().ToSequentialGuid();
        private readonly string _subject = "Test subject";
        private readonly string _template = "Test template";
        private readonly string _payload = "Test payload";
        private readonly string _type = "Test type";
        protected override IEnumerable<IEvent> Given()
        {
            yield break;
        }

        protected override void When()
        {
            Aggregate.Queue(_aggregateId, _subject, _payload, _template, _type, _recipientId);
        }

        [Then]
        public void ShouldContainOneUncommitedEvent()
        {
            Aggregate.GetUncommittedEvents().Should().HaveCount(1);
        }

        [Then]
        public void ShouldContainUncommitedEmailQueuedEvent()
        {
            var events = Aggregate.GetUncommittedEvents();

            events.Single().Should().BeOfType<EmailQueued>();
        }

        [Then]
        public void ShouldContainUncommitedEmailQueuedEventWithCorrectAggregateId()
        {
            var events = Aggregate.GetUncommittedEvents();

            var @event = events.OfType<EmailQueued>().Single();

            @event.AggregateId.Should().Be(_aggregateId);
        }

        [Then]
        public void ShouldContainUncommitedEmailQueuedEventWithCorrectSubject()
        {
            var events = Aggregate.GetUncommittedEvents();

            var @event = events.OfType<EmailQueued>().Single();

            @event.Subject.Should().Be(_subject);
        }

        [Then]
        public void ShouldContainUncommitedEmailQueuedEventWithCorrectPayload()
        {
            var events = Aggregate.GetUncommittedEvents();

            var @event = events.OfType<EmailQueued>().Single();

            @event.Payload.Should().Be(_payload);
        }

        [Then]
        public void ShouldContainUncommitedEmailQueuedEventWithCorrectTemplate()
        {
            var events = Aggregate.GetUncommittedEvents();
            var @event = events.OfType<EmailQueued>().Single();

            @event.Template.Should().Be(_template);
        }

        [Then]
        public void ShouldContainUncommitedEmailQueuedEventWithCorrectType()
        {
            var events = Aggregate.GetUncommittedEvents();
            var @event = events.OfType<EmailQueued>().Single();

            @event.Type.Should().BeEquivalentTo(_type);
        }

        [Then]
        public void ShouldContainUncommitedEmailQueuedEventWithCorrectRecipientId()
        {
            var events = Aggregate.GetUncommittedEvents();
            var @event = events.OfType<EmailQueued>().Single();

            @event.RecipientId.Should().Be(_recipientId);
        }

        [Then]
        public void ShouldContainUncommitedEmailQueuedEventWithCorrectVersion()
        {
            var events = Aggregate.GetUncommittedEvents();
            var @event = events.OfType<EmailQueued>().Single();

            @event.Version.Should().Be(1);
        }

        [Then]
        public void ShouldContainAggregateWithCorrectId()
        {
            Aggregate.Id.Should().Be(_aggregateId);
        }

        [Then]
        public void ShouldContainAggregateWithCorrectVersion()
        {
            Aggregate.Version.Should().Be(1);
        }

        [Then]
        public void ShouldContainStateWithCorrectSubject()
        {
            Aggregate.GetState().Subject.Should().Be(_subject);
        }

        [Then]
        public void ShouldContainStateWithCorrectPayload()
        {
            Aggregate.GetState().Payload.Should().Be(_payload);
        }

        [Then]
        public void ShouldContainStateWithCorrectTemplate()
        {
            Aggregate.GetState().Template.Should().Be(_template);
        }

        [Then]
        public void ShouldContainStateWithCorrectType()
        {
            Aggregate.GetState().Type.Should().Be(_type);
        }

        [Then]
        public void ShouldContainStateWithCorrectRecipientId()
        {
            Aggregate.GetState().RecipientId.Should().Be(_recipientId);
        }

        [Then]
        public void ShouldContainStateWithCorrectStatus()
        {
            Aggregate.GetState().Status.Should().Be(EmailStatus.Pending);
        }

        [Then]
        public void ShouldContainStateWithCorrectAttempts()
        {
            Aggregate.GetState().Attempts.Should().Be(0);
        }
    }
}
