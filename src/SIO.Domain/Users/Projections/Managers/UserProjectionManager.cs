using Microsoft.Extensions.Logging;
using SIO.Infrastructure.Projections;
using SIO.IntegrationEvents.Users;

namespace SIO.Domain.Users.Projections.Managers
{
    internal sealed class UserProjectionManager : ProjectionManager<User>
    {
        private readonly IEnumerable<IProjectionWriter<User>> _projectionWriters;

        public UserProjectionManager(ILogger<UserProjectionManager> logger,
            IEnumerable<IProjectionWriter<User>> projectionWriters) : base(logger)
        {
            if( projectionWriters == null)
                throw new ArgumentNullException(nameof(projectionWriters));

            _projectionWriters = projectionWriters;

            Handle<UserRegistered>(HandleAsync);
        }

        public async Task HandleAsync(UserRegistered @event, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(UserProjectionManager)}.{nameof(HandleAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            await Task.WhenAll(_projectionWriters.Select(pw => pw.AddAsync(@event.Subject, () => new User
            {
                Subject = @event.Subject,
                Email = @event.Email
            }, cancellationToken)));
        }

        public override async Task ResetAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(UserProjectionManager)}.{nameof(ResetAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            await Task.WhenAll(_projectionWriters.Select(pw => pw.ResetAsync(cancellationToken)));
        }
    }
}
