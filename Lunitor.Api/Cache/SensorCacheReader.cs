using Lunitor.HardwareMonitorAPI.Models;
using Lunitor.Shared.Dto;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Lunitor.Api.Cache
{
    public class SensorCacheReader : ISensorCacheReader
    {
        private readonly IDatabase _cache;
        private readonly IServer _server;

        public SensorCacheReader(IDatabase cache, IServer server)
        {
            _cache = cache;
            _server = server;
        }

        public IEnumerable<SensorReadingDto> GetAll()
        {
            var sensorReadings = new List<SensorReadingDto>();

            var keys = _server.Keys();

            foreach (var key in keys)
            {
                sensorReadings.AddRange(_cache.ListRange(key, 0, -1)
                    .Select(sr => JsonSerializer.Deserialize<SensorReadingDto>(sr)));
            }

            return sensorReadings;
        }
    }
}
