using Lunitor.Shared.Dto;
using Lunitor.Shared.Json;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Text.Json;

namespace Lunitor.DataReader.Cache
{
    class SensorCacheCleaner : ISensorCacheCleaner
    {
        private readonly IDatabase _cache;
        private readonly IServer _server;

        private readonly int ExpirationTimeInMinutes;
        private readonly JsonSerializerOptions _serializerOptions;

        public SensorCacheCleaner(IDatabase cache, IServer server, IConfiguration configuration)
        {
            _cache = cache;
            _server = server;

            ExpirationTimeInMinutes = configuration.GetValue<int>("CacheWriter:ExpirationTime");

            _serializerOptions = new JsonSerializerOptions();
            _serializerOptions.Converters.Add(new FloatStringConverter());
        }

        public void Clean()
        {
            var currentTime = DateTime.Now;
            var keys = _server.Keys();

            foreach (var key in keys)
            {
                foreach (var expiredSensorReading in _cache.ListRange(key, 0, -1)
                    .Select(sr => JsonSerializer.Deserialize<SensorReadingDto>(sr, _serializerOptions))
                    .Where(sr => sr.TimeStamp < currentTime.AddMinutes(-ExpirationTimeInMinutes))
                    .Select(sr => JsonSerializer.Serialize(sr, _serializerOptions)))
                {
                    _cache.ListRemove(key, expiredSensorReading);
                }
            }
        }
    }
}
