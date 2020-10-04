using System;
using System.Threading.Tasks;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Projections;
using SIO.Domain.Emails.Events;
using SIO.Domain.Emails.Projections;

namespace SIO.Domain.Projections.Emails
{
    public sealed class EmailFailureProjection : Projection<EmailFailure>
    {
        public EmailFailureProjection(IProjectionWriter<EmailFailure> writer) : base(writer)
        {
            Handles<EmailFailed>(HandleAsync);
        }

        public async Task HandleAsync(EmailFailed @event)
        {
            var id = Guid.NewGuid().ToSequentialGuid();
            await _writer.Add(id, () =>
            {
                return new EmailFailure
                {
                    Id = id,
                    EmailId = @event.AggregateId,
                    Error = @event.Error
                };
            });
        }
    }
}
