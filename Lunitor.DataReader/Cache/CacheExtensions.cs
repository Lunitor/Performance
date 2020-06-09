using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Lunitor.DataReader.Cache
{
    public static class CacheExtensions
    {
        public static void AddCache(this IServiceCollection services, string connection)
        {
            services.AddSingleton(sp =>
            {
                ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(connection);

                return connectionMultiplexer.GetDatabase();
            });

            services.AddSingleton<ISensorReadingCache, SensorReadingCache>();
        }
    }
}
