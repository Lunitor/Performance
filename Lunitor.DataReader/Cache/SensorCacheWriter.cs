using Ardalis.GuardClauses;
using Lunitor.Shared.Dto;
using Lunitor.Shared.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Lunitor.DataReader.Cache
{
    class SensorCacheWriter : ISensorCacheWriter
    {
        private readonly IDatabase _cache;

        private readonly JsonSerializerOptions _serializerOptions;

        public SensorCacheWriter(IDatabase cache)
        {
            _cache = cache;

            _serializerOptions = new JsonSerializerOptions();
            _serializerOptions.Converters.Add(new FloatStringConverter());
        }

        public void Add(IEnumerable<SensorReadingDto> sensorReadings)
        {
            Guard.Against.Null(sensorReadings, nameof(sensorReadings));

            // Add new sensor readings
            var sensorReadingsByTypes = sensorReadings.GroupBy(sr => new { HardwareName = sr.Hardware.Name, SensorName = sr.Sensor.Name }, sr => sr);

            foreach (var group in sensorReadingsByTypes)
            {
                var listKey = $"{group.Key.HardwareName}.{group.Key.SensorName}";
                var readings = group.Select(sr => JsonSerializer.Serialize(sr, _serializerOptions)).ToArray().ToRedisValueArray();

                _cache.ListLeftPush(listKey, readings);
            }
        }
    }
}
