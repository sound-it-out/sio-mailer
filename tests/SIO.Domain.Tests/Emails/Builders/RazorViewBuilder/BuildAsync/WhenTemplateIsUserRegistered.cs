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
    public class WhenTemplateIsUserRegistered : MailerApplicationSpecification<RazorViewBuilderStartup, string>
    {

        private readonly MailModel<UserRegistered> _payload = new MailModel<UserRegistered>()
        {
            Event = new UserRegistered(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), "test@test.com", "test", "test", "test"),
            Metadata = new Dictionary<string, object>()
        };
        private readonly string _template = $"~/Emails/Templates/{nameof(UserRegistered)}/{nameof(UserRegistered)}Email.cshtml";

        public WhenTemplateIsUserRegistered(MailerWebApplicationFactory<RazorViewBuilderStartup> mailerWebApplicationFactory) : base(mailerWebApplicationFactory)
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
