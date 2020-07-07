using Lunitor.Api.Cache;
using Lunitor.Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lunitor.Api.Services
{
    public class SensorReadingService : ISensorReadingService
    {
        private readonly ISensorCacheReader _sensorCache;

        public SensorReadingService(ISensorCacheReader sensorCache)
        {
            _sensorCache = sensorCache;
        }

        public IEnumerable<SensorReadingDto> Get(DateTime? from = null, DateTime? to = null, float criticalValuePercent = 1.0f)
        {
            if (from.HasValue && !to.HasValue)
                return _sensorCache.GetAll().Where(sensorReading => sensorReading.TimeStamp >= from
                    && ReachedCriticalValue(criticalValuePercent, sensorReading));
            else if (to.HasValue && !from.HasValue)
                return _sensorCache.GetAll().Where(sensorReading => sensorReading.TimeStamp <= to
                    && ReachedCriticalValue(criticalValuePercent, sensorReading));
            else if (from.HasValue && to.HasValue)
                return _sensorCache.GetAll().Where(sensorReading => sensorReading.TimeStamp >= from && sensorReading.TimeStamp <= to
                    && ReachedCriticalValue(criticalValuePercent, sensorReading));
            else if (criticalValuePercent < 1.0)
                return _sensorCache.GetAll().Where(sensorReading => ReachedCriticalValue(criticalValuePercent, sensorReading));

            return _sensorCache.GetAll();
        }

        private static bool ReachedCriticalValue(float? criticalValuePercent, SensorReadingDto sensorReading)
        {
            return sensorReading.Value >= criticalValuePercent * (sensorReading.Sensor.MaxValue ?? float.MaxValue);
        }
    }
}
