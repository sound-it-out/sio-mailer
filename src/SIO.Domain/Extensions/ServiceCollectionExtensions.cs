using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIO.Domain.Emails;
using SIO.Domain.Emails.Builders;
using SIO.Domain.Emails.Mailers;
using SIO.Domain.Emails.Processors;
using SIO.Domain.Emails.Serialization;

namespace SIO.Domain.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomain(this IServiceCollection source)
        {
            source.AddHostedService<BackgroundMailer>();
            source.AddTransient<IRazorViewBuilder, RazorViewBuilder>();
            source.AddTransient<IMailMessageBuilder, MailMessageBuilder>();
            source.AddTransient<IEmailProcessor, EmailProcessor>();
            source.AddTransient<IPayloadSerializer, PayloadSerializer>();
            source.AddTransient<IPayloadDeserializer, PayloadDeserializer>();

            return source;
        }

        public static IServiceCollection AddDomainConfiguration(this IServiceCollection source, IConfiguration configuration)
        {
            source.Configure<EmailOptions>(configuration.GetSection("Email"));
            source.Configure<SmtpOptions>(configuration.GetSection("Smtp"));
            source.Configure<MailerOptions>(configuration.GetSection("Mailer"));

            return source;
        }
    }
}
