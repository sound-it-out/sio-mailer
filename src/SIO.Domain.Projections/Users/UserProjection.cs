using System.Threading.Tasks;
using OpenEventSourcing.Projections;
using SIO.Domain.Users.Events;
using SIO.Domain.Users.Projections;

namespace SIO.Domain.Projections.Users
{
    public sealed class UserProjection : Projection<User>
    {
        public UserProjection(IProjectionWriter<User> writer) : base(writer)
        {
            Handles<UserRegistered>(HandleAsync);
            Handles<UserEmailChanged>(HandleAsync);
            Handles<UserPasswordTokenGenerated>(HandleAsync);
        }

        public async Task HandleAsync(UserRegistered @event)
        {
            await _writer.Add(@event.AggregateId, () =>
            {
                return new User
                {
                    Id = @event.AggregateId,
                    Email = @event.Email,
                    ActivationToken = @event.ActivationToken,
                    PasswordToken = ""
                };
            });
        }

        public async Task HandleAsync(UserEmailChanged @event)
        {
            await _writer.Update(@event.AggregateId, user =>
            {
                user.Email = @event.Email;
            });
        }

        public async Task HandleAsync(UserPasswordTokenGenerated @event)
        {
            await _writer.Update(@event.AggregateId, user =>
            {
                user.PasswordToken = @event.Token;
            });
        }
    }
}
