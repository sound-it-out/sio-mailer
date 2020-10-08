using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.Projections;
using SIO.Domain.Emails;
using SIO.Testing.Specifications;

namespace SIO.Domain.Projections.Tests.Emails.EmailQueueProjection
{
    public abstract class EmailQueueProjectionSpecification<TProjection> : ProjectionSpecification<TProjection>
            where TProjection : IProjection
    {
        protected readonly int _maxRetries = 5;

        protected override void BuildServices(IServiceCollection services)
        {
            services.Configure<EmailOptions>(options =>
            {
                options.MaxRetries = _maxRetries;
            });

            base.BuildServices(services);
        }
    }
}
