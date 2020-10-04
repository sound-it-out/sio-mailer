using Microsoft.Extensions.DependencyInjection;
using SIO.Domain.Emails.Builders;
using SIO.Domain.Emails.Notifiers;

namespace SIO.Domain.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomain(this IServiceCollection source)
        {
            source.AddHostedService<BackgroundMailer>();
            source.AddTransient<IRazorViewBuilder, RazorViewBuilder>();

            return source;
        }
    }
}
