using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
    public abstract class EventHandlerSpecification<TEvent> : IAsyncLifetime
        where TEvent : IEvent
    {
        protected abstract TEvent Given();
        protected abstract Task When();
        protected Exception Exception { get; private set; }
        protected ExceptionMode ExceptionMode { get; set; }

        protected readonly IServiceProvider _serviceProvider;
        protected readonly IEventDispatcher _eventDispatcher;

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

            services.RemoveAll<ICommandDispatcher>();

            services.AddSingleton<IEventBusPublisher, FakeEventBusPublisher>();
            services.AddSingleton<ICommandDispatcher, FakeCommandDispatcher>();
        }

        public EventHandlerSpecification()
        {
            var services = new ServiceCollection();

            BuildServices(services);

            _serviceProvider = services.BuildServiceProvider();
            _eventDispatcher = _serviceProvider.GetRequiredService<IEventDispatcher>();
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
                await _eventDispatcher.DispatchAsync(Given());
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
