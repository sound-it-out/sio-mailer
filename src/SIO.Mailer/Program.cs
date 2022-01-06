using SIO.Infrastructure.EntityFrameworkCore.Extensions;

namespace SIO.Mailer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var env = host.Services.GetRequiredService<IHostEnvironment>();

            if (env.IsDevelopment())
                await host.RunProjectionMigrationsAsync();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
