using System.Net.Mail;
using System.Threading.Tasks;
using SIO.Domain.Emails.Aggregates;
using SIO.Domain.Emails.Builders;

namespace SIO.Testing.Fakes.Notifications.Builders
{
    public sealed class FakeMailMessageBuilder : IMailMessageBuilder
    {

        public Task<MailMessage> BuildAsync(EmailState email)
        {
            return Task.FromResult(new MailMessage());
        }
    }
}
