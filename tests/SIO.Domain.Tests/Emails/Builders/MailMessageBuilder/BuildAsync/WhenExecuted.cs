using System;
using System.Net.Mail;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using SIO.Domain.Emails.Aggregates;
using SIO.Domain.Emails.Builders;
using SIO.Domain.Emails.Processors;
using SIO.Domain.Emails.Serialization;
using SIO.Domain.Extensions;
using SIO.Domain.Users.Events;
using SIO.Domain.Users.Projections;
using SIO.Testing.Attributes;
using SIO.Testing.Extensions;
using SIO.Testing.Fakes.Notifications.Builders;
using SIO.Testing.Specifications;

namespace SIO.Domain.Tests.Emails.Builders.MailMessageBuilder.BuildAsync
{
    public class WhenExecuted : Specification<MailMessage>
    {
        private readonly int _attempts = 0;
        private readonly string _subject = "Test subject";
        private readonly Guid _recipientId = Guid.NewGuid().ToSequentialGuid();
        private readonly string _recepientEmail = "test@test.com";
        private readonly string _fromEmail = "noreply@test.com";
        private readonly string _body = "Test body";
        private readonly EmailStatus _status = EmailStatus.Pending;

        private string _payload;
        private string _template;
        private string _type;

        protected override void BuildServices(IServiceCollection services)
        {
            base.BuildServices(services);

            services.AddOpenEventSourcing()
                .AddCommands()
                .AddEvents()
                .AddQueries()
                .AddJsonSerializers();

            services.AddInMemoryDatabase()
                .AddDomain()
                .AddLogging();

            services.Configure<SmtpOptions>(options =>
            {
                options.From = _fromEmail;
            });

            services.RemoveAll<IRazorViewBuilder>();
            services.AddSingleton<IRazorViewBuilder>(new FakeRazorViewBuilder(_body));
        }

        protected override Task<MailMessage> Given()
        {
            var builder = _serviceProvider.GetRequiredService<IMailMessageBuilder>();
            return builder.BuildAsync(new EmailState()
            {
                Attempts = _attempts,
                Payload = _payload,
                RecipientId = _recipientId,
                Status = _status,
                Subject = _subject,
                Template = _template,
                Type = _type
            });
        }

        protected override async Task When()
        {
            var payloadSerializer = _serviceProvider.GetRequiredService<IPayloadSerializer>();
            var projectionDbContextFactory = _serviceProvider.GetRequiredService<IProjectionDbContextFactory>();

            using(var context = projectionDbContextFactory.Create())
            {
                await context.Set<User>().AddAsync(new User
                {
                    ActivationToken = "",
                    Email = _recepientEmail,
                    Id = _recipientId,
                    PasswordToken = ""
                });

                await context.SaveChangesAsync();
            }

            _payload = payloadSerializer.Serialize(new UserRegistered(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), _recepientEmail, "test", "test", "test"));
            _template = $"~/Emails/Templates/{nameof(UserRegistered)}/{nameof(UserRegistered)}Email.cshtml";
            _type = typeof(UserRegistered).FullName;
        }

        [Then]
        public void MailMessageShouldHaveCorrectBody()
        {
            Result.Body.Should().Be(_body);
        }

        [Then]
        public void MailMessageShouldHaveCorrectSubject()
        {
            Result.Subject.Should().Be(_subject);
        }

        [Then]
        public void MailMessageShouldHaveCorrectTo()
        {
            Result.To.Should().BeEquivalentTo(new MailAddress[] { new MailAddress(_recepientEmail) });
        }

        [Then]
        public void MailMessageShouldHaveCorrectFrom()
        {
            Result.From.Should().Be(_fromEmail);
        }
    }
}
