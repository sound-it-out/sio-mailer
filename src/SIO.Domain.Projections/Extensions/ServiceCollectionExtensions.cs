using System;
using Microsoft.Extensions.DependencyInjection;
using SIO.Domain.Projections.Emails;

namespace SIO.Domain.Projections.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjections(this IServiceCollection source)
        {
            source.AddScoped<EmailQueueProjection>();
            source.AddScoped<EmailFailureProjection>();

            source.AddHostedService<PollingProjector<EmailQueueProjection>>();
            source.AddHostedService<PollingProjector<EmailFailureProjection>>();

            return source;
        }
    }
}
