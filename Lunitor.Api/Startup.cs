using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.SystemTextJson;
using Lunitor.Api.Cache;
using Lunitor.Api.GraphQL;
using Lunitor.Shared.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();
            services.AddScoped<SensorReadingQuery>();
            services.AddScoped<SensorReadingSchema>();
            services.AddGraphQL(options =>
            {
                options.ExposeExceptions = true;
            })
            .AddSystemTextJson(deserializerSettings => { }, serializerSettings => { })
            .AddGraphTypes(ServiceLifetime.Scoped);

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
