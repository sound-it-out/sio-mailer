using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SIO.Domain.Emails.Templates.ResetPassword;
using SIO.Domain.Emails.Templates.UserRegistered;
using SIO.Domain.Users.Projections;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;

namespace SIO.Domain.Emails.Templates.UserPasswordTokenGenerated
{
    internal sealed class UserPasswordTokenGeneratedMailMessageBuilder : MailMessageBuilder<IntegrationEvents.Users.UserPasswordTokenGenerated>
    {
        private readonly UrlOptions _urlOptions;
        private readonly ISIOProjectionDbContextFactory _projectionDbContextFactory;

        private bool _isVerified;
        private string _email;
        private string _firstName;

        public UserPasswordTokenGeneratedMailMessageBuilder(ILogger<MailMessageBuilder<IntegrationEvents.Users.UserPasswordTokenGenerated>> logger,
            IRazorViewBuilder razorViewBuilder,
            IOptionsSnapshot<SmtpOptions> smtpOptionsSnapshot,
            IOptionsSnapshot<UrlOptions> urlOptionsSnapshot,
            ISIOProjectionDbContextFactory projectionDbContextFactory) : base(logger, razorViewBuilder, smtpOptionsSnapshot)
        {
            if (urlOptionsSnapshot == null)
                throw new ArgumentException(nameof(urlOptionsSnapshot));
            if (projectionDbContextFactory == null)
                throw new ArgumentException(nameof(projectionDbContextFactory));

            _urlOptions = urlOptionsSnapshot.Value;
            _projectionDbContextFactory = projectionDbContextFactory;
        }

        protected override Task<string[]> GenerateRecipentsAsync(IntegrationEvents.Users.UserPasswordTokenGenerated @event) => Task.FromResult(new string[] { _email });
        protected override Task<string> GenerateSubjectAsync(IntegrationEvents.Users.UserPasswordTokenGenerated @event) => Task.FromResult(_isVerified ? "Reset your password" : "Thanks for registering, please verify your email");

        protected override async Task InitializeAsync(IntegrationEvents.Users.UserPasswordTokenGenerated @event)
        {
            using (var context = _projectionDbContextFactory.Create())
            {
                var user = await context.Set<User>().FindAsync(@event.Subject);

                if (user == null)
                    throw new Exception($"User with id:{@event.Subject} does not exist");

                _isVerified = user.Verified;
                _email = user.Email;
                _firstName = user.FirstName;
            }

            await base.InitializeAsync(@event);
        }

        protected override async Task<string> GenerateBodyAsync(IntegrationEvents.Users.UserPasswordTokenGenerated @event)
        {
            if(_isVerified)
            {
                return await _razorViewBuilder.BuildAsync(
                    "ResetPassword",
                    new ResetPasswordPayLoad(_email, _firstName, Convert.ToBase64String(Encoding.UTF8.GetBytes(@event.Token)), _urlOptions)
                );
            }

            return await _razorViewBuilder.BuildAsync(
                nameof(IntegrationEvents.Users.UserRegistered),
                new UserRegisteredPayLoad(_email, _firstName, Convert.ToBase64String(Encoding.UTF8.GetBytes(@event.Token)), _urlOptions)
            );
        }
    }
}
