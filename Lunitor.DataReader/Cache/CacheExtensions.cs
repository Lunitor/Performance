using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Lunitor.DataReader.Cache
{
    public static class CacheExtensions
    {
        public static void AddCache(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();

                ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis"));

                return connectionMultiplexer.GetDatabase();
            });

            services.AddSingleton<ISensorReadingCache, SensorReadingCache>();
        }
    }
}
