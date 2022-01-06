using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SIO.Domain.Emails.Commands;
using SIO.Infrastructure;
using SIO.Infrastructure.Commands;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.EntityFrameworkCore.Entities;
using SIO.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SIO.Domain.Emails.Services
{
    internal sealed class EventProcessor : IHostedService
    {
        private Task _executingTask;
        private CancellationTokenSource StoppingCts { get; set; }
        private readonly IServiceScope _scope;
        private readonly IEventStore _eventStore;
        private readonly ILogger<EventProcessor> _logger;
        private readonly IOptionsSnapshot<EventProcessorOptions> _options;
        private readonly ISIOProjectionDbContextFactory _projectionDbContextFactory;
        private readonly string _name;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly HashSet<string> _eventsToProcess;

        public EventProcessor(IServiceScopeFactory serviceScopeFactory,
            ILogger<EventProcessor> logger)
        {
            if (serviceScopeFactory == null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _scope = serviceScopeFactory.CreateScope();
            _logger = logger;
            _eventStore = _scope.ServiceProvider.GetRequiredService<IEventStore>();
            _options = _scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<EventProcessorOptions>>();
            _projectionDbContextFactory = _scope.ServiceProvider.GetRequiredService<ISIOProjectionDbContextFactory>();
            _commandDispatcher = _scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

            _name = typeof(EventProcessor).FullName;
            _eventsToProcess = new HashSet<string>(EventHelper.EmailEvents.Select(e => e.FullName));
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EventProcessor)}.{nameof(StartAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            _logger.LogInformation($"{nameof(EventProcessor)} starting");
            StoppingCts = new();

            _executingTask = ExecuteAsync(StoppingCts.Token);

            _logger.LogInformation($"{nameof(EventProcessor)} started");

            if (_executingTask.IsCompleted)
                return _executingTask;

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EventProcessor)}.{nameof(StopAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            _logger.LogInformation($"{nameof(EventProcessor)} stopping");

            if (_executingTask == null)
                return;

            try
            {
                StoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
                _logger.LogInformation($"{nameof(EventProcessor)} stopped");
            }
        }

        private async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EventProcessor)}.{nameof(ExecuteAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using (var context = _projectionDbContextFactory.Create())
            {
                var state = await context.ProjectionStates.FindAsync(_name);

                if (state == null)
                {
                    state = new ProjectionState
                    {
                        Name = _name,
                        CreatedDate = DateTimeOffset.UtcNow,
                        Position = 1
                    };

                    context.ProjectionStates.Add(state);

                    await context.SaveChangesAsync(cancellationToken);
                }

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await context.Entry(state).ReloadAsync();

                        var page = await _eventStore.GetEventsAsync(state.Position);
                        var correlationId = CorrelationId.New();
                        foreach (var @event in page.Events.Where(e => _eventsToProcess.Contains(e.Payload.GetType().FullName)))
                        {
                            var test = @event.Payload.GetType().FullName;
                            await _commandDispatcher.DispatchAsync(new QueueEmailCommand(
                                subject: Subject.New(),
                                correlationId: correlationId,
                                version: 0,
                                Actor.Unknown,
                                @event.ScheduledPublication,
                                @event: @event
                            ));
                        }

                        if (state.Position == page.Offset)
                        {
                            await Task.Delay(_options.Value.Interval);
                        }
                        else
                        {
                            state.Position = page.Offset;
                            state.LastModifiedDate = DateTimeOffset.UtcNow;

                            await context.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, $"Process '{typeof(EventProcessor).Name}' failed at postion '{state.Position}' due to an unexpected error. See exception details for more information.");
                        break;
                    }
                }
            }
        }
    }
}
