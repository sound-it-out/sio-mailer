using Microsoft.Extensions.Logging;
using SIO.Domain.Emails.Events;
using SIO.Infrastructure;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.Projections;

namespace SIO.Domain.Emails.Projections.Managers
{
    internal sealed class EmailFailureProjectionManager : ProjectionManager<EmailFailure>
    {
        private readonly IEnumerable<IProjectionWriter<EmailFailure>> _projectionWriters;
        private readonly ISIOProjectionDbContextFactory _projectionDbContextFactory;

        public EmailFailureProjectionManager(ILogger<ProjectionManager<EmailFailure>> logger,
            IEnumerable<IProjectionWriter<EmailFailure>> projectionWriters) : base(logger)
        {
            if (projectionWriters == null)
                throw new ArgumentNullException(nameof(projectionWriters));

            _projectionWriters = projectionWriters;

            Handle<EmailFailed>(HandleAsync);
        }

        public async Task HandleAsync(EmailFailed @event, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EmailQueueProjectionManager)}.{nameof(HandleAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            await Task.WhenAll(_projectionWriters.Select(pw => pw.AddAsync(@event.Subject, () => new EmailFailure
            {
                Id = Subject.New(),
                Subject = @event.Subject,
                Error = @event.Error,
                Body = @event.Body,
                Recipients = @event.Recipients
            }, cancellationToken)));
        }

        public override async Task ResetAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EmailFailureProjectionManager)}.{nameof(ResetAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            await Task.WhenAll(_projectionWriters.Select(pw => pw.ResetAsync(cancellationToken)));
        }
    }
}
