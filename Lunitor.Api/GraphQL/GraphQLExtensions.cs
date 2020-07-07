using GraphQL;
using GraphQL.Server;
using GraphQL.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;

namespace Lunitor.Api.GraphQL
{
    public static class GraphQLExtensions
    {
        public static void AddCustomGraphQL(this IServiceCollection services)
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
        }
    }
}
