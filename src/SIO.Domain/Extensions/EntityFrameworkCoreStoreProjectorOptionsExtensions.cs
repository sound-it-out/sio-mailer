using Microsoft.Extensions.Configuration;
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
        public static void WithDomainProjections(this EntityFrameworkCoreStoreProjectorOptions options, IConfiguration configuration)
            => options.WithProjection<EmailFailure, EmailFailureProjectionManager, SIOMailerStoreDbContext>(o => o.Interval = configuration.GetValue<int>("EmailFailure:Interval"))
                .WithProjection<EmailQueue, EmailQueueProjectionManager, SIOMailerStoreDbContext>(o => o.Interval = configuration.GetValue<int>("EmailQueue:Interval"))
                .WithProjection<User, UserProjectionManager, SIOStoreDbContext>(o => o.Interval = configuration.GetValue<int>("User:Interval"));
    }
}
