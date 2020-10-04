using System.Net.Mail;
using System.Threading.Tasks;

namespace SIO.Domain.Emails.Processors
{
    public interface IEmailProcessor
    {
        Task ProcessAsync(MailMessage mailMessage);
    }
}
