using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenEventSourcing.Events;

namespace SIO.Testing.Fakes.Events
{
    public class FakeEventBusPublisher : IEventBusPublisher
    {
        public ConcurrentBag<IEvent> Events { get; }

        public FakeEventBusPublisher()
        {
            Events = new ConcurrentBag<IEvent>();
        }

        public Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            Events.Add(@event);
            return Task.CompletedTask;
        }

        public Task PublishAsync(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
                Events.Add(@event);
            return Task.CompletedTask;
        }
    }
}
