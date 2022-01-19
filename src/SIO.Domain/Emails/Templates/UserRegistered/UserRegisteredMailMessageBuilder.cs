using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SIO.Domain.Emails.Templates.UserRegistered
{
    internal sealed class UserRegisteredMailMessageBuilder : MailMessageBuilder<IntegrationEvents.Users.UserRegistered>
    {
        private readonly UrlOptions _urlOptions;

        public UserRegisteredMailMessageBuilder(ILogger<MailMessageBuilder<IntegrationEvents.Users.UserRegistered>> logger,
            IRazorViewBuilder razorViewBuilder,
            IOptionsSnapshot<SmtpOptions> smtpOptionsSnapshot,
            IOptionsSnapshot<UrlOptions> urlOptionsSnapshot) : base(logger, razorViewBuilder, smtpOptionsSnapshot)
        {
            if (urlOptionsSnapshot == null)
                throw new ArgumentException(nameof(urlOptionsSnapshot));

            _urlOptions = urlOptionsSnapshot.Value;
        }

        protected override Task<string[]> GenerateRecipentsAsync(IntegrationEvents.Users.UserRegistered @event) => Task.FromResult(new string[] { @event.Email });
        protected override Task<string> GenerateSubjectAsync(IntegrationEvents.Users.UserRegistered @event) => Task.FromResult("Thanks for registering, please verify your email");

        protected override Task<string> GenerateBodyAsync(IntegrationEvents.Users.UserRegistered @event)
            => _razorViewBuilder.BuildAsync(
                nameof(IntegrationEvents.Users.UserRegistered),
                new UserRegisteredPayLoad(@event.Email, @event.FirstName, Convert.ToBase64String(Encoding.UTF8.GetBytes(@event.ActivationToken)), _urlOptions)
            );
    }
}
