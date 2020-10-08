using System.Net.Mail;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using netDumbster.smtp;
using SIO.Domain.Emails.Processors;
using SIO.Domain.Extensions;
using SIO.Testing.Attributes;
using SIO.Testing.Fixtures;
using SIO.Testing.Specifications;
using Xunit;

namespace SIO.Domain.Tests.Emails.Processors.EmailProcessor.ProcessAsync
{
    public class WhenExecuted : Specification, IClassFixture<SmtpServerFixture>
    {
        private readonly string _subject = "Test subject";
        private readonly string _body = "Test body";
        private readonly string _to = "test@test.test";
        private readonly string _from = "noreply@test.test";
        private SimpleSmtpServer _smtpServer;

        protected override Task Given()
        {
            var emailProcessor = _serviceProvider.GetRequiredService<IEmailProcessor>();
            return emailProcessor.ProcessAsync(new MailMessage(_from, _to, _subject, _body));
        }

        protected override Task When()
        {
            //_smtpServer = SimpleSmtpServer.Start(25);
            return Task.CompletedTask;
        }

        protected override void BuildServices(IServiceCollection services)
        {
            services.AddDomain();

            services.Configure<SmtpOptions>(options =>
            {
                options.DeliveryMethod = SmtpDeliveryMethod.Network;
                options.Host = "localhost";
                options.Port = 25;
                options.From = _from;
                options.UseSsl = true;
            });

            base.BuildServices(services);
        }

        public WhenExecuted(SmtpServerFixture smtpServerFixture)
        {
            _smtpServer = smtpServerFixture.Server;
        }

        [Then]
        public void MailShouldBeSentWithCorrectFrom()
        {
            _smtpServer.ReceivedEmail[0].FromAddress.Address.Should().Be(_from);
        }

        [Then]
        public void MailShouldBeSentWithCorrectTo()
        {
            _smtpServer.ReceivedEmail[0].ToAddresses[0].Address.Should().Be(_to);
        }

        [Then]
        public void MailShouldBeSentWithCorrectSubject()
        {
            _smtpServer.ReceivedEmail[0].Headers["Subject"].Should().Be(_subject);
        }

        [Then]
        public void MailShouldBeSentWithCorrectBody()
        {
            _smtpServer.ReceivedEmail[0].MessageParts[0].BodyData.Should().Be(_body);
        }
    }
}
