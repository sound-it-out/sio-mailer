using System;
using System.Collections.Generic;
using FluentAssertions;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using SIO.Domain.Emails.Events;
using SIO.Domain.Emails.Projections;
using SIO.Testing.Abstractions;
using SIO.Testing.Attributes;

namespace SIO.Domain.Projections.Tests.Emails.EmailQueueProjection
{
    public class WhenEmailQueued : EmailQueueProjectionSpecification<Projections.Emails.EmailQueueProjection>
    {
        private readonly Guid _aggregateId = Guid.NewGuid().ToSequentialGuid();
        private readonly Guid _recipientId = Guid.NewGuid().ToSequentialGuid();
        private readonly string _subject = "Test subject";
        private readonly string _template = "Test template";
        private readonly string _payload = "Test payload";
        private readonly string _type = "Test type";

        protected override IEnumerable<IEvent> Given()
        {
            yield return new EmailQueued(_aggregateId, 1, _recipientId, _subject, _template, _payload, _type);
        }

        [Then]
        public void EmailQueueShouldNotBeNull()
        {
            var emailQueue = Context.Find<EmailQueue>(_aggregateId);
            emailQueue.Should().NotBeNull();
        }

        [Then]
        public void EmailQueueShouldHaveCorrectId()
        {
            var emailQueue = Context.Find<EmailQueue>(_aggregateId);
            emailQueue.Id.Should().Be(_aggregateId);
        }

        [Then]
        public void EmailQueueShouldHaveCorrectRecipientId()
        {
            var emailQueue = Context.Find<EmailQueue>(_aggregateId);
            emailQueue.RecipientId.Should().Be(_recipientId);
        }

        [Then]
        public void EmailQueueShouldHaveCorrectSubject()
        {
            var emailQueue = Context.Find<EmailQueue>(_aggregateId);
            emailQueue.Subject.Should().Be(_subject);
        }

        [Then]
        public void EmailQueueShouldHaveCorrectTemplate()
        {
            var emailQueue = Context.Find<EmailQueue>(_aggregateId);
            emailQueue.Template.Should().Be(_template);
        }

        [Then]
        public void EmailQueueShouldHaveCorrectPayload()
        {
            var emailQueue = Context.Find<EmailQueue>(_aggregateId);
            emailQueue.Payload.Should().Be(_payload);
        }

        [Then]
        public void EmailQueueShouldHaveCorrectType()
        {
            var emailQueue = Context.Find<EmailQueue>(_aggregateId);
            emailQueue.Type.Should().Be(_type);
        }

        [Then]
        public void EmailQueueShouldHaveCorrectStatus()
        {
            var emailQueue = Context.Find<EmailQueue>(_aggregateId);
            emailQueue.Status.Should().Be(EmailStatus.Pending);
        }

        [Then]
        public void EmailQueueShouldHaveCorrectAttempts()
        {
            var emailQueue = Context.Find<EmailQueue>(_aggregateId);
            emailQueue.Attempts.Should().Be(0);
        }

        [Then]
        public void EmailQueueShouldHaveCorrectVersion()
        {
            var emailQueue = Context.Find<EmailQueue>(_aggregateId);
            emailQueue.Version.Should().Be(1);
        }
    }
}
