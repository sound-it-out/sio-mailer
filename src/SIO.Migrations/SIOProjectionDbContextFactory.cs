using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;

namespace SIO.Migrations
{
    internal class MigrationsSIOProjectionDbContextFactory : IDesignTimeDbContextFactory<SIOProjectionDbContext>
    {
        public SIOProjectionDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SIOProjectionDbContext>();
            optionsBuilder.UseSqlServer("Server=.,1456;Initial Catalog=sio-api-projections;User Id=sa;Password=1qaz-pl,", b => b.MigrationsAssembly("SIO.Migrations"));

            return new SIOProjectionDbContext(optionsBuilder.Options);
        }
    }
}
