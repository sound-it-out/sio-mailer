using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using SIO.Domain.Extensions;
using SIO.Domain.Tests.Notifications.Notifiers;
using SIO.Testing.Extensions;
using SIO.Testing.Fakes.Events;
using Xunit;

namespace SIO.Testing.Abstractions
{
    public abstract class CommandHandlerSpecification<TCommand> : IAsyncLifetime
        where TCommand : ICommand
    {
        protected abstract TCommand Given();
        protected abstract Task When();
        protected Exception Exception { get; private set; }
        protected ExceptionMode ExceptionMode { get; set; }

        protected readonly IServiceProvider _serviceProvider;
        protected readonly ICommandDispatcher _commandDispatcher;

        protected virtual void BuildServices(IServiceCollection services)
        {
            services.AddOpenEventSourcing()
                .AddCommands()
                .AddEvents()
                .AddQueries()
                .AddJsonSerializers();

            services.AddDomain();
            services.AddInMemoryDatabase();
            services.AddLogging();
            services.AddSingleton<IEventBusPublisher, FakeEventBusPublisher>();
        }

        public CommandHandlerSpecification()
        {
            var services = new ServiceCollection();

            BuildServices(services);

            _serviceProvider = services.BuildServiceProvider();
            _commandDispatcher = _serviceProvider.GetServices<ICommandDispatcher>().First(c => c.GetType() != typeof(FakeCommandDispatcher));
        }

        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual async Task InitializeAsync()
        {
            await When();

            try
            {
                await _commandDispatcher.DispatchAsync(Given());
            }
            catch (Exception e)
            {
                if (ExceptionMode == ExceptionMode.Record)
                    Exception = e;
                else
                    throw;
            }
        }
    }
}
