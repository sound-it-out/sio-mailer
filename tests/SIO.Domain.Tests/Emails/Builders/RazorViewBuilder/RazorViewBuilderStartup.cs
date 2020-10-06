﻿using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using SIO.Domain.Extensions;
using SIO.Domain.Projections.Extensions;
using SIO.Infrastructure.Extensions;
using SIO.Mailer;
using SIO.Testing.Abstractions;
using SIO.Testing.Extensions;

namespace SIO.Domain.Tests.Emails.Builders.RazorViewBuilder
{
    public sealed class RazorViewBuilderStartup : BaseStartup
    {
        public RazorViewBuilderStartup(IHostEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

        public override void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            return;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddOpenEventSourcing()
                .AddCommands()
                .AddQueries()
                .AddEvents()
                .AddJsonSerializers();

            services.AddInfrastructure()
                .AddInMemoryDatabase()
                .AddInMemoryEventBus()
                .AddDomain()
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
    }
}