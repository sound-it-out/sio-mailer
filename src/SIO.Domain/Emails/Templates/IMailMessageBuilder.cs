using System.Net.Mail;
using SIO.Infrastructure.Events;

namespace SIO.Domain.Emails.Templates
{
    public interface IMailMessageBuilder<TEvent>
        where TEvent : IEvent
    {
        Task<MailMessage> BuildAsync(TEvent @event);
    }

    public interface IMailMessageBuilder
    {
        Task<MailMessage> BuildAsync(IEvent @event);
    }
}
