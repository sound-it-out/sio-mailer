using SIO.Domain.Extensions;
using SIO.Mailer.Extensions;

namespace SIO.Mailer
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddInfrastructure(_configuration)
                .AddDomain(_configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            if (!_env.IsDevelopment())
                app.UseHttpsRedirection();

            app.UseRouting();
            app.UseStaticFiles();
        }
    }
}
