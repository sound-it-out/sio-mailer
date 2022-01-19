using SIO.Domain.Emails.Projections;
using SIO.Domain.Emails.Projections.Managers;
using SIO.Domain.Users.Projections;
using SIO.Domain.Users.Projections.Managers;
using SIO.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.EntityFrameworkCore.Extensions;

namespace SIO.Domain.Extensions
{
    public static class EntityFrameworkCoreStoreProjectorOptionsExtensions
    {
        public static void WithDomainProjections(this EntityFrameworkCoreStoreProjectorOptions options)
            => options.WithProjection<EmailFailure, EmailFailureProjectionManager, SIOMailerStoreDbContext>(o => o.Interval = 5000)
                .WithProjection<EmailQueue, EmailQueueProjectionManager, SIOMailerStoreDbContext>(o => o.Interval = 5000)
                .WithProjection<User, UserProjectionManager, SIOStoreDbContext>(o => o.Interval = 5000);
    }
}
