using GraphQL.Server.Ui.Playground;
using Lunitor.Api.GraphQL;
using Lunitor.Core.Interfaces;
using Lunitor.Infrastructure;
using Lunitor.Infrastructure.Cache;
using Lunitor.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lunitor.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<PeriodicReader>();
            services.AddHarwareMonitorAPI();

            services.AddSingleton<ISensorReadingRepository, SensorReadingRepository>();

            services.AddCustomGraphQL();

            services.AddCache(Configuration.GetConnectionString("Redis"));

            services.AddControllers()
                .AddJsonOptions(configure => configure.JsonSerializerOptions.Converters.Add(new FloatStringConverter()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseGraphQL<SensorReadingSchema>();
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
