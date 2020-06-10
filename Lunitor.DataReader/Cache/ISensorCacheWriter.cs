using Lunitor.Shared.Dto;
using System.Collections.Generic;

namespace Lunitor.DataReader.Cache
{
    interface ISensorCacheWriter
    {
        void Add(IEnumerable<SensorReadingDto> sensorReadings);
    }
}
