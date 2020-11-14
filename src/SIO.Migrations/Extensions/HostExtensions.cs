using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;

namespace SIO.Migrations.Extensions
{
    public static class HostExtensions
    {
        public static async Task<IHost> SeedDatabaseAsync(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<IDbContextFactory>().Create())
                    await context.Database.MigrateAsync();
                using (var context = scope.ServiceProvider.GetRequiredService<OpenEventSourcing.EntityFrameworkCore.DbContexts.IProjectionDbContextFactory>().Create())
                    await context.Database.MigrateAsync();
            }

            return host;
        }
    }
}
