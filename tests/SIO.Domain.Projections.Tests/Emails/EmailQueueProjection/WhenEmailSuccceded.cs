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
    public class WhenEmailSuccceded : EmailQueueProjectionSpecification<Projections.Emails.EmailQueueProjection>
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
            yield return new EmailSucceded(_aggregateId, 2);
        }

        [Then]
        public void EmailQueueShouldBeNull()
        {
            var emailQueue = Context.Find<EmailQueue>(_aggregateId);
            emailQueue.Should().BeNull();
        }
    }
}
