using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;
using SIO.Migrations;

await Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddTransient<IDesignTimeDbContextFactory<SIOProjectionDbContext>, MigrationsSIOProjectionDbContextFactory>();
                })
                .Build()
                .RunAsync();
