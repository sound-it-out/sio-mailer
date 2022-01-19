using SIO.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.EntityFrameworkCore.Extensions;
using SIO.Mailer;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    })
    .Build();

var env = host.Services.GetRequiredService<IHostEnvironment>();

if (env.IsDevelopment())
{
    await host.RunProjectionMigrationsAsync();
    await host.RunStoreMigrationsAsync<SIOMailerStoreDbContext>();
}    

await host.RunAsync();
