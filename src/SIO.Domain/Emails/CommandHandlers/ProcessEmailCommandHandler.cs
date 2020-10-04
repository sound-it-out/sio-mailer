using System;
using System.Linq;
using System.Threading.Tasks;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Domain;
using OpenEventSourcing.Events;
using SIO.Domain.Emails.Aggregates;
using SIO.Domain.Emails.Builders;
using SIO.Domain.Emails.Commands;
using SIO.Domain.Emails.Processors;

namespace SIO.Domain.Emails.CommandHandlers
{
    public class ProcessEmailCommandHandler : ICommandHandler<ProcessEmailCommand>
    {
        private readonly IAggregateRepository _aggregateRepository;
        private readonly IEventBusPublisher _eventBusPublisher;
        private readonly IEmailProcessor _emailProcessor;
        private readonly IMailMessageBuilder _mailMessageBuilder;

        public ProcessEmailCommandHandler(IAggregateRepository aggregateRepository,
            IEventBusPublisher eventBusPublisher,
            IEmailProcessor emailProcessor,
            IMailMessageBuilder mailMessageBuilder)
        {
            if (aggregateRepository == null)
                throw new ArgumentNullException(nameof(aggregateRepository));
            if (eventBusPublisher == null)
                throw new ArgumentNullException(nameof(eventBusPublisher));
            if (emailProcessor == null)
                throw new ArgumentNullException(nameof(emailProcessor));
            if (mailMessageBuilder == null)
                throw new ArgumentNullException(nameof(mailMessageBuilder));

            _aggregateRepository = aggregateRepository;
            _eventBusPublisher = eventBusPublisher;
            _emailProcessor = emailProcessor;
            _mailMessageBuilder = mailMessageBuilder;
        }
        public async Task ExecuteAsync(ProcessEmailCommand command)
        {
            var aggregate = await _aggregateRepository.GetAsync<Email, EmailState>(command.AggregateId);

            try
            {
                var message = await _mailMessageBuilder.BuildAsync(aggregate.GetState());
                await _emailProcessor.ProcessAsync(message);

                aggregate.MarkAsSuccess();
            }
            catch (Exception e)
            {
                aggregate.MarkAsFail(e.Message);
            }

            var events = aggregate.GetUncommittedEvents();

            foreach (var @event in events)
                @event.UpdateFrom(command);

            events = events.ToList();

            await _aggregateRepository.SaveAsync(aggregate, command.Version);
            await _eventBusPublisher.PublishAsync(events);
        }
    }
}
