using System;
using OpenEventSourcing.Commands;

namespace SIO.Domain.Emails.Commands
{
    public class ProcessEmailCommand : Command
    {
        public ProcessEmailCommand(Guid aggregateId, Guid correlationId, int version, string userId) : base(aggregateId, correlationId, version, userId)
        {
        }
    }
}
