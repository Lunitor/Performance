using Ardalis.GuardClauses;
using Lunitor.Shared.Dto;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Lunitor.DataReader.Cache
{
    class SensorCacheWriter : ISensorCacheWriter
    {
        private readonly IDatabase _cache;

        public SensorCacheWriter(IDatabase cache)
        {
            _cache = cache;
        }

        public void Add(IEnumerable<SensorReadingDto> sensorReadings)
        {
            Guard.Against.Null(sensorReadings, nameof(sensorReadings));

            var sensorReadingsByTypes = sensorReadings.GroupBy(sr => new { HardwareName = sr.Hardware.Name, SensorName = sr.Sensor.Name }, sr => sr);

            foreach (var group in sensorReadingsByTypes)
            {
                var listKey = $"{group.Key.HardwareName}.{group.Key.SensorName}";
                var readings = group.Select(sr => JsonSerializer.Serialize(sr)).ToArray().ToRedisValueArray();

                _cache.ListLeftPush(listKey, readings);
            }
        }
    }
}
