using Ardalis.GuardClauses;
using Lunitor.Core.Interfaces.Cache;
using Lunitor.Core.Models;
using Lunitor.Shared;
using Lunitor.Shared.Dto;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Lunitor.Infrastructure.Cache
{
    public class SensorCacheMutator : ISensorCacheMutator
    {
        private readonly IDatabase _cache;
        private readonly IServer _server;

        private readonly int ExpirationTimeInMinutes;
        private readonly JsonSerializerOptions _serializerOptions;

        public SensorCacheMutator(IDatabase cache, IServer server, IConfiguration configuration)
        {
            _cache = cache;
            _server = server;

            ExpirationTimeInMinutes = configuration.GetValue<int>(ConfigurationConstants.ExpirationTimeKey);

            _serializerOptions = new JsonSerializerOptions();
            _serializerOptions.Converters.Add(new FloatStringConverter());
        }

        public void Add(IEnumerable<SensorReading> sensorReadings)
        {
            Guard.Against.Null(sensorReadings, nameof(sensorReadings));

            // Add new sensor readings
            var sensorReadingsByTypes = sensorReadings.GroupBy(sr => new { HardwareName = sr.Hardware.Name, SensorName = sr.Sensor.Name }, sr => sr);

            foreach (var group in sensorReadingsByTypes)
            {
                var listKey = $"{group.Key.HardwareName}.{group.Key.SensorName}";
                var readings = group.Select(sr => JsonSerializer.Serialize(sr.Map(), _serializerOptions)).ToArray().ToRedisValueArray();

                _cache.ListLeftPush(listKey, readings);
            }
        }

        public void Clean()
        {
            var currentTime = DateTime.UtcNow;
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
