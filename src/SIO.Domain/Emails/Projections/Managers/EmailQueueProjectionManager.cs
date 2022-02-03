using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SIO.Domain.Emails.Events;
using SIO.Domain.Emails.Services;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.Projections;

namespace SIO.Domain.Emails.Projections.Managers
{
    internal sealed class EmailQueueProjectionManager : ProjectionManager<EmailQueue>
    {
        private readonly IEnumerable<IProjectionWriter<EmailQueue>> _projectionWriters;
        private readonly ISIOProjectionDbContextFactory _projectionDbContextFactory;
        private readonly IOptionsMonitor<EmailPublisherOptions> _options;

        public EmailQueueProjectionManager(ILogger<ProjectionManager<EmailQueue>> logger,
            IEnumerable<IProjectionWriter<EmailQueue>> projectionWriters,
            ISIOProjectionDbContextFactory projectionDbContextFactory,
            IOptionsMonitor<EmailPublisherOptions> options) : base(logger)
        {
            if (projectionWriters == null)
                throw new ArgumentNullException(nameof(projectionWriters));
            if (projectionDbContextFactory == null)
                throw new ArgumentNullException(nameof(projectionDbContextFactory));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _projectionWriters = projectionWriters;
            _projectionDbContextFactory = projectionDbContextFactory;
            _options = options;

            Handle<EmailQueued>(HandleAsync);
            Handle<EmailFailed>(HandleAsync);
            Handle<EmailSucceded>(HandleAsync);
        }

        public async Task HandleAsync(EmailQueued @event, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EmailQueueProjectionManager)}.{nameof(HandleAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            await Task.WhenAll(_projectionWriters.Select(pw => pw.AddAsync(@event.Subject, () => new EmailQueue
            {
                Attempts = 0,
                Subject = @event.Subject,                
                PublicationDate = @event.PublicationDate,
                Payload = @event.Payload
            }, cancellationToken)));
        }

        public async Task HandleAsync(EmailFailed @event, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EmailQueueProjectionManager)}.{nameof(HandleAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using (var context = _projectionDbContextFactory.Create())
            {
                var email = await context.Set<EmailQueue>().FindAsync(@event.Subject);
                if (email?.Attempts == _options.CurrentValue.MaxRetries)
                {
                    await Task.WhenAll(_projectionWriters.Select(pw => pw.RemoveAsync(@event.Subject)));
                }
                else
                {
                    await Task.WhenAll(_projectionWriters.Select(pw => pw.UpdateAsync(@event.Subject, epq =>
                    {
                        epq.Attempts++;
                    })));
                }
            }
        }

        public async Task HandleAsync(EmailSucceded @event, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EmailQueueProjectionManager)}.{nameof(HandleAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            await Task.WhenAll(_projectionWriters.Select(pw => pw.RemoveAsync(@event.Subject)));
        }

        public override async Task ResetAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EmailQueueProjectionManager)}.{nameof(ResetAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            await Task.WhenAll(_projectionWriters.Select(pw => pw.ResetAsync(cancellationToken)));
        }
    }
}
