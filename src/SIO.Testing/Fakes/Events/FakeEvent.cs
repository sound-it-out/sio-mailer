using System;
using OpenEventSourcing.Events;

namespace SIO.Testing.Fakes.Events
{
    public class FakeEvent : Event
    {
        public FakeEvent(Guid aggregateId, int version) : base(aggregateId, version)
        {
        }
    }
}
