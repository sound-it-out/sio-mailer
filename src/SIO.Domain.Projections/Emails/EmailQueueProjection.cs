using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.Projections;
using SIO.Domain.Emails;
using SIO.Domain.Emails.Events;
using SIO.Domain.Emails.Projections;

namespace SIO.Domain.Projections.Emails
{
    public sealed class EmailQueueProjection : Projection<EmailQueue>
    {
        private readonly IProjectionDbContextFactory _projectionDbContextFactory;
        private readonly int _maxRetries;
        public EmailQueueProjection(IProjectionWriter<EmailQueue> writer, 
            IProjectionDbContextFactory projectionDbContextFactory,
            IOptions<EmailOptions> emailOptions) : base(writer)
        {
            if (projectionDbContextFactory == null)
                throw new ArgumentNullException(nameof(projectionDbContextFactory));
            if (emailOptions == null)
                throw new ArgumentNullException(nameof(emailOptions));

            _projectionDbContextFactory = projectionDbContextFactory;
            _maxRetries = emailOptions.Value.MaxRetries;

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
                    Subject = @event.Subject,
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
                var email = await context.Set<EmailQueue>().FindAsync(@event.AggregateId);
                if(email.Attempts == _maxRetries)
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
