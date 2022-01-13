using Microsoft.EntityFrameworkCore;
using SIO.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.EntityFrameworkCore.Migrations;

namespace SIO.Migrations
{
    public static class MigratorExtensions
    {
        public static Migrator AddContexts(this Migrator migrator)
            => migrator.WithDbContext<SIOProjectionDbContext>(o => o.UseSqlServer("Server=.,1453;Initial Catalog=sio-mailer-projections;User Id=sa;Password=1qaz-pl,", b => b.MigrationsAssembly("SIO.Migrations")))
            .WithDbContext<SIOMailerStoreDbContext>(o => o.UseSqlServer("Server=.,1453;Initial Catalog=sio-mailer-store;User Id=sa;Password=1qaz-pl,", b => b.MigrationsAssembly("SIO.Migrations")));
    }
}
