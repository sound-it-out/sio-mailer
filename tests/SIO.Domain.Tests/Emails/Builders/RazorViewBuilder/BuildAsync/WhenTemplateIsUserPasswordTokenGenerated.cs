using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SIO.Domain.Emails.Builders;
using SIO.Domain.Users.Events;
using SIO.Testing.Abstractions;
using SIO.Testing.Attributes;
using SIO.Testing.Specifications;

namespace SIO.Domain.Tests.Emails.Builders.RazorViewBuilder.BuildAsync
{
    public class WhenTemplateIsUserPasswordTokenGenerated : MailerApplicationSpecification<RazorViewBuilderStartup, string>
    {

        private readonly MailModel<UserPasswordTokenGenerated> _payload = new MailModel<UserPasswordTokenGenerated>()
        {
            Event = new UserPasswordTokenGenerated(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), "test"),
            Metadata = new Dictionary<string, object>()
        };
        private readonly string _template = $"~/Emails/Templates/{nameof(UserPasswordTokenGenerated)}/{nameof(UserPasswordTokenGenerated)}Email.cshtml";

        public WhenTemplateIsUserPasswordTokenGenerated(MailerWebApplicationFactory<RazorViewBuilderStartup> mailerWebApplicationFactory) : base(mailerWebApplicationFactory)
        {
        }

        protected override Task<string> Given()
        {
            var builder = _mailerWebApplicationFactory.Services.CreateScope().ServiceProvider.GetRequiredService<IRazorViewBuilder>();
            return builder.BuildAsync(_template, _payload);
        }

        protected override Task When()
        {
            return Task.CompletedTask; 
        }

        [Then]
        public void ViewShouldNotBeNull()
        {
            Result.Should().NotBeNullOrEmpty();
        }
    }
}
