using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using SIO.Domain.Emails.Events;
using SIO.Domain.Emails.Projections;
using SIO.Testing.Abstractions;
using SIO.Testing.Attributes;

namespace SIO.Domain.Projections.Tests.Emails.EmailFailureProjection
{
    public class WhenEmailFailed : ProjectionSpecification<Projections.Emails.EmailFailureProjection>
    {
        private readonly Guid _aggregateId = Guid.NewGuid().ToSequentialGuid();
        private readonly string _error = "Test error";

        protected override IEnumerable<IEvent> Given()
        {
            yield return new EmailFailed(_aggregateId, 0, _error);
        }

        [Then]
        public void EmailFailureShouldNotBeNull()
        {
            var emailFailure = Context.Set<EmailFailure>().Where(ef => ef.EmailId == _aggregateId).FirstOrDefault();
            emailFailure.Should().NotBeNull();
        }

        [Then]
        public void EmailFailureIdShouldNotBeNull()
        {
            var emailFailure = Context.Set<EmailFailure>().Where(ef => ef.EmailId == _aggregateId).FirstOrDefault();
            emailFailure.Id.Should().NotBeEmpty();
        }

        [Then]
        public void EmailFailureShouldHaveCorrectEmailId()
        {
            var emailFailure = Context.Set<EmailFailure>().Where(ef => ef.EmailId == _aggregateId).FirstOrDefault();
            emailFailure.EmailId.Should().Be(_aggregateId);
        }

        [Then]
        public void EmailFailureShouldHaveCorrectError()
        {
            var emailFailure = Context.Set<EmailFailure>().Where(ef => ef.EmailId == _aggregateId).FirstOrDefault();
            emailFailure.Error.Should().Be(_error);
        }
    }
}
