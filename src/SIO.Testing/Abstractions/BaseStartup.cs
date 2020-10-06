using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SIO.Testing.Abstractions
{
    public abstract class BaseStartup
    {
        protected readonly IHostEnvironment _env;
        protected readonly IConfiguration _configuration;

        public BaseStartup(IHostEnvironment env,
            IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        public abstract void ConfigureServices(IServiceCollection services);
        public abstract void Configure(IApplicationBuilder app, IHostEnvironment env);
    }
}
