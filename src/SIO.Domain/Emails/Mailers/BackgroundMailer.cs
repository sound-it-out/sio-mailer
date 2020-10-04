using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Commands;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Queries;
using SIO.Domain.Emails.Commands;
using SIO.Domain.Emails.Projections;
using SIO.Domain.Emails.Queries;

namespace SIO.Domain.Emails.Mailers
{
    public class BackgroundMailer : IHostedService
    {
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private readonly ILogger<BackgroundMailer> _logger;
        private readonly IServiceScope _scope;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly MailerOptions _mailerOptions;

        private Task _executingTask;

        public BackgroundMailer(ILogger<BackgroundMailer> logger,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<MailerOptions> mailerOptions)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceScopeFactory == null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            if (mailerOptions == null)
                throw new ArgumentNullException(nameof(mailerOptions));

            _logger = logger;
            _scope = serviceScopeFactory.CreateScope();
            _mailerOptions = mailerOptions.Value;
            _commandDispatcher = _scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();
            _queryDispatcher = _scope.ServiceProvider.GetRequiredService<IQueryDispatcher>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            if (_executingTask.IsCompleted)
                return _executingTask;

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _scope.Dispose();
            if (_executingTask == null)
                return;

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
        }

        private async Task PollAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var emailsQueryResult = await _queryDispatcher.DispatchAsync(new GetEmailsQuery(Guid.NewGuid().ToSequentialGuid(), Guid.Empty.ToString(), _mailerOptions.PageSize));

                    foreach (var email in emailsQueryResult.Emails)
                    {
                        await _commandDispatcher.DispatchAsync(new ProcessEmailCommand(
                            aggregateId: email.Id,
                            correlationId: Guid.NewGuid(),
                            version: email.Version,
                            userId: Guid.Empty.ToString()
                        ));
                    }

                    await Task.Delay(_mailerOptions.Interval);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, $"Projection '{GetType().Name}' failed due to an unexpected error. See exception details for more information.");
                    break;
                }
            }
        }

        protected Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => PollAsync(cancellationToken));
        }
    }
}
