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
    public class WhenEmailFailed : AggregateSpecification<Domain.Emails.Aggregates.Email, EmailState>
    {
        private readonly Guid _aggregateId = Guid.NewGuid().ToSequentialGuid();
        private readonly Guid _recipientId = Guid.NewGuid().ToSequentialGuid();
        private readonly string _subject = "Test subject";
        private readonly string _template = "Test template";
        private readonly string _payload = "Test payload";
        private readonly string _type = "Test type";
        private readonly string _error = "Test error";
        protected override IEnumerable<IEvent> Given()
        {
            yield return new EmailQueued(_aggregateId, 1, _recipientId, _subject, _template, _payload, _type);
        }

        protected override void When()
        {
            Aggregate.MarkAsFail(_error);
        }

        [Then]
        public void ShouldContainOneUncommitedEvent()
        {
            Aggregate.GetUncommittedEvents().Should().HaveCount(1);
        }

        [Then]
        public void ShouldContainUncommitedEmailFailedEvent()
        {
            var events = Aggregate.GetUncommittedEvents();

            events.Single().Should().BeOfType<EmailFailed>();
        }

        [Then]
        public void ShouldContainUncommitedEmailFailedEventWithCorrectAggregateId()
        {
            var events = Aggregate.GetUncommittedEvents();

            var @event = events.OfType<EmailFailed>().Single();

            @event.AggregateId.Should().Be(_aggregateId);
        }

        [Then]
        public void ShouldContainUncommitedEmailFailedEventWithCorrectError()
        {
            var events = Aggregate.GetUncommittedEvents();

            var @event = events.OfType<EmailFailed>().Single();

            @event.Error.Should().Be(_error);
        }

        [Then]
        public void ShouldContainUncommitedEmailFailedEventWithCorrectVersion()
        {
            var events = Aggregate.GetUncommittedEvents();
            var @event = events.OfType<EmailFailed>().Single();

            @event.Version.Should().Be(2);
        }

        [Then]
        public void ShouldContainAggregateWithCorrectId()
        {
            Aggregate.Id.Should().Be(_aggregateId);
        }

        [Then]
        public void ShouldContainAggregateWithCorrectVersion()
        {
            Aggregate.Version.Should().Be(2);
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
            Aggregate.GetState().Status.Should().Be(EmailStatus.Failed);
        }

        [Then]
        public void ShouldContainStateWithCorrectAttempts()
        {
            Aggregate.GetState().Attempts.Should().Be(1);
        }
    }
}
