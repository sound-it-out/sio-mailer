using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using SIO.Mailer;

namespace SIO.Testing.Abstractions
{
    public class MailerWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
        where TStartup: BaseStartup
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseStartup<TStartup>();
            base.ConfigureWebHost(builder);
        }
    }
}
