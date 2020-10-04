using System.Net.Mail;
using System.Threading.Tasks;
using SIO.Domain.Emails.Aggregates;

namespace SIO.Domain.Emails.Builders
{
    public interface IMailMessageBuilder
    {
        Task<MailMessage> BuildAsync(EmailState email);
    }
}
