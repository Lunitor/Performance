﻿using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Lunitor.Api.Cache
{
    public static class CacheExtensions
    {
        public static void AddCache(this IServiceCollection services, string connection)
        {
            var connectionMx = ConnectionMultiplexer.Connect(connection);

            services.AddSingleton(sp => connectionMx.GetDatabase());
            services.AddSingleton(sp => connectionMx.GetServer(connectionMx.Configuration));

            services.AddSingleton<ISensorReadingCache, SensorReadingCache>();
        }
    }
}
