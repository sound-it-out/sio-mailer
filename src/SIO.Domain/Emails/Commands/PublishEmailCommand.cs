using SIO.Infrastructure;
using SIO.Infrastructure.Commands;

namespace SIO.Domain.Emails.Commands
{
    public class PublishEmailCommand : Command
    {
        public PublishEmailCommand(string subject, CorrelationId? correlationId, int version, Actor actor) : base(subject, correlationId, version, actor)
        {
        }
    }
}
