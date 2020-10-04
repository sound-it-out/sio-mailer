using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenEventSourcing.Azure.ServiceBus.Extensions;
using OpenEventSourcing.EntityFrameworkCore.SqlServer;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using SIO.Domain.Extensions;
using SIO.Domain.Projections.Extensions;
using SIO.Domain.Translation.Events;
using SIO.Infrastructure.Extensions;

namespace SIO.Notifier
{
    public class Startup
    {
        private readonly IHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public Startup(IHostEnvironment env,
            IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOpenEventSourcing()
                .AddEntityFrameworkCoreSqlServer(options => {
                    options.MigrationsAssembly("SIO.Migrations");
                })
                .AddAzureServiceBus(options =>
                {
                    options.UseConnection(_configuration.GetValue<string>("Azure:ServiceBus:ConnectionString"))
                    .UseTopic(e =>
                    {
                        e.WithName(_configuration.GetValue<string>("Azure:ServiceBus:Topic"));
                    })
                    .AddSubscription(s =>
                    {
                        s.UseName(_configuration.GetValue<string>("Azure:ServiceBus:Subscription"));
                        s.ForEvent<TranslationFailed>();
                        s.ForEvent<TranslationSucceded>();                        
                    });
                })
                .AddCommands()
                .AddEvents()
                .AddJsonSerializers();

            services.AddInfrastructure()
                .AddDomain()
                .AddProjections();

            services.AddMvcCore();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
        }
    }
}
