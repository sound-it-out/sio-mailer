using System;
using System.Threading.Tasks;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.Projections;
using SIO.Domain.Emails.Events;
using SIO.Domain.Emails.Projections;

namespace SIO.Domain.Projections.Emails
{
    public sealed class EmailQueueProjection : Projection<EmailQueue>
    {
        private readonly IProjectionDbContextFactory _projectionDbContextFactory;
        public EmailQueueProjection(IProjectionWriter<EmailQueue> writer, IProjectionDbContextFactory projectionDbContextFactory) : base(writer)
        {
            if (projectionDbContextFactory == null)
                throw new ArgumentNullException(nameof(projectionDbContextFactory));

            _projectionDbContextFactory = projectionDbContextFactory;

            Handles<EmailQueued>(HandleAsync);
            Handles<EmailFailed>(HandleAsync);
            Handles<EmailSucceded>(HandleAsync);
        }

        public async Task HandleAsync(EmailQueued @event)
        {
            await _writer.Add(@event.AggregateId, () =>
            {
                return new EmailQueue
                {
                    Id = @event.AggregateId,
                    Attempts = 0,
                    Status = EmailStatus.Pending,
                    RecipientId = @event.RecipientId,
                    Payload = @event.Payload,
                    Template = @event.Template,
                    Type = @event.Type,
                    Version = @event.Version,
                };
            });
        }

        public async Task HandleAsync(EmailFailed @event)
        {
            using(var context = _projectionDbContextFactory.Create())
            {
                var notification = await context.Set<EmailQueue>().FindAsync(@event.AggregateId);
                if(notification.Attempts == 5)
                {
                    await _writer.Remove(@event.AggregateId);
                }
                else
                {
                    await _writer.Update(@event.AggregateId, n =>
                    {
                        n.Attempts++;
                        n.Status = EmailStatus.Failed;
                        n.Version = @event.Version;
                    });
                }
            }
        }

        public async Task HandleAsync(EmailSucceded @event)
        {
            await _writer.Remove(@event.AggregateId);
        }
    }
}
