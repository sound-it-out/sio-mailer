using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenEventSourcing.Azure.ServiceBus.Extensions;
using OpenEventSourcing.EntityFrameworkCore.SqlServer;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using SIO.Domain.Extensions;
using SIO.Domain.Projections.Extensions;
using SIO.Domain.Users.Events;
using SIO.Infrastructure.Extensions;

namespace SIO.Mailer
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
                        s.ForEvent<UserRegistered>();
                        s.ForEvent<UserPasswordTokenGenerated>();                        
                    });
                })
                .AddCommands()
                .AddQueries()
                .AddEvents()
                .AddJsonSerializers();

            services.AddInfrastructure()
                .AddDomain()
                .AddDomainConfiguration(_configuration)
                .AddProjections();

            services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            {
                options.AddEmailTemplates();
            });

            var assembly = typeof(Startup).GetTypeInfo().Assembly;
            services.AddMvc().AddRazorRuntimeCompilation()
                            .AddApplicationPart(assembly)
                            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
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
