using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SIO.Domain.Emails.Commands;
using SIO.Domain.Emails.Projections;
using SIO.Infrastructure;
using SIO.Infrastructure.Commands;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;

namespace SIO.Domain.Emails.Services
{
    internal sealed class EmailPublisher : IHostedService
    {
        private Task _executingTask;
        private CancellationTokenSource StoppingCts { get; set; }
        private readonly IServiceScope _scope;
        private readonly ILogger<EmailPublisher> _logger;
        private readonly IOptionsMonitor<EmailPublisherOptions> _options;
        private readonly ISIOProjectionDbContextFactory _projectionDbContextFactory;
        private readonly string _name;
        private readonly ICommandDispatcher _commandDispatcher;

        public EmailPublisher(IServiceScopeFactory serviceScopeFactory,
            IOptionsMonitor<EmailPublisherOptions> options,
            ILogger<EmailPublisher> logger)
        {
            if (serviceScopeFactory == null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _scope = serviceScopeFactory.CreateScope();
            _logger = logger;
            _options = options;
            _projectionDbContextFactory = _scope.ServiceProvider.GetRequiredService<ISIOProjectionDbContextFactory>();
            _commandDispatcher = _scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

            _name = typeof(EmailPublisher).FullName;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EmailPublisher)}.{nameof(StartAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            _logger.LogInformation($"{nameof(EmailPublisher)} starting");
            StoppingCts = new();

            _executingTask = ExecuteAsync(StoppingCts.Token);

            _logger.LogInformation($"{nameof(EmailPublisher)} started");

            if (_executingTask.IsCompleted)
                return _executingTask;

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EmailPublisher)}.{nameof(StopAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            _logger.LogInformation($"{nameof(EmailPublisher)} stopping");

            if (_executingTask == null)
                return;

            try
            {
                StoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
                _logger.LogInformation($"{nameof(EmailPublisher)} stopped");
            }
        }

        private async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EmailPublisher)}.{nameof(ExecuteAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (var context = _projectionDbContextFactory.Create())
                    {
                        var eventsInQueue = await context.Set<EmailQueue>()
                            .AsQueryable()
                            .Where(epq => !epq.PublicationDate.HasValue || epq.PublicationDate <= DateTimeOffset.UtcNow)
                            .Take(50)
                            .Select(epq => epq.Subject)
                            .ToArrayAsync();

                        var correlationId = CorrelationId.New();

                        foreach (var @event in eventsInQueue)
                        {
                            await _commandDispatcher.DispatchAsync(new PublishEmailCommand(
                                subject: @event,
                                correlationId: correlationId,
                                version: 0,
                                Actor.Unknown
                            ));
                        }

                        if (eventsInQueue.Count() == 0)
                            await Task.Delay(_options.CurrentValue.Interval);
                        else
                            await context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, $"Process '{typeof(EmailPublisher).Name}' failed due to an unexpected error. See exception details for more information.");
                    break;
                }
            }
        }
    }
}
