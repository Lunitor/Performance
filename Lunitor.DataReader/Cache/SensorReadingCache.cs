using Ardalis.GuardClauses;
using Lunitor.HardwareMonitorAPI.Models;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Lunitor.DataReader.Cache
{
    class SensorReadingCache : ISensorReadingCache
    {
        private readonly IDatabase _cache;

        public SensorReadingCache(IDatabase cache)
        {
            _cache = cache;
        }

        public void Add(IEnumerable<SensorReading> sensorReadings)
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
