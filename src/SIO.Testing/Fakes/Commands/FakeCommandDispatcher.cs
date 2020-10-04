using System.Collections.Concurrent;
using System.Threading.Tasks;
using OpenEventSourcing.Commands;

namespace SIO.Domain.Tests.Notifications.Notifiers
{
    public class FakeCommandDispatcher : ICommandDispatcher
    {
        public ConcurrentBag<ICommand> Commands { get; set; }

        public FakeCommandDispatcher()
        {
            Commands = new ConcurrentBag<ICommand>();
        }

        public Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            Commands.Add(command);
            return Task.CompletedTask;
        }
    }
}
