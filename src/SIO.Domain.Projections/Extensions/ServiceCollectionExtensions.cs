using System;
using Microsoft.Extensions.DependencyInjection;
using SIO.Domain.Projections.Emails;
using SIO.Domain.Projections.Users;

namespace SIO.Domain.Projections.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjections(this IServiceCollection source)
        {
            source.AddScoped<EmailQueueProjection>();
            source.AddScoped<EmailFailureProjection>();
            source.AddScoped<UserProjection>();

            source.AddHostedService<PollingProjector<EmailQueueProjection>>();
            source.AddHostedService<PollingProjector<EmailFailureProjection>>();
            source.AddHostedService<PollingProjector<UserProjection>>();

            return source;
        }
    }
}
