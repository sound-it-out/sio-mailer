using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIO.Domain.Emails;
using SIO.Domain.Emails.CommandHandlers;
using SIO.Domain.Emails.Commands;
using SIO.Domain.Emails.Serialization;
using SIO.Domain.Emails.Services;
using SIO.Domain.Emails.Templates;
using SIO.Domain.Emails.Templates.UserPasswordTokenGenerated;
using SIO.Domain.Emails.Templates.UserRegistered;
using SIO.Infrastructure.Commands;
using SIO.Infrastructure.Extensions;
using SIO.IntegrationEvents.Users;

namespace SIO.Domain.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICommandHandler<QueueEmailCommand>, QueueEmailCommandHandler>();
            services.AddScoped<ICommandHandler<PublishEmailCommand>, PublishEmailCommandHandler>();
            services.AddScoped<IMailMessageBuilder, MailMessageBuilder>();
            services.AddScoped<IMailMessageBuilder<UserRegistered>, UserRegisteredMailMessageBuilder>();
            services.AddScoped<IMailMessageBuilder<UserPasswordTokenGenerated>, UserPasswordTokenGeneratedMailMessageBuilder>();
            services.AddScoped<IRazorViewBuilder, RazorViewBuilder>();
            services.AddScoped<ICommandHandler<PublishEmailCommand>, PublishEmailCommandHandler>();
            services.AddScoped<IPayloadDeserializer, PayloadDeserializer>();
            services.AddScoped<IPayloadSerializer, PayloadSerializer>();
            services.AddHostedService<EventProcessor>();
            services.AddHostedService<EmailPublisher>();
            services.Configure<EventProcessorOptions>(o => o.Interval = 300);
            services.Configure<EmailPublisherOptions>(o => o.Interval = 300);
            services.Configure<EmailOptions>(o => o.MaxRetries = 5);
            services.Configure<SmtpOptions>(configuration.GetSection("SMTP"));
            services.Configure<UrlOptions>(configuration.GetSection("URL"));
            return services;
        }
    }
}
