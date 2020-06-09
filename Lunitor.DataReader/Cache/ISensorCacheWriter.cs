using Lunitor.HardwareMonitorAPI.Models;
using System.Collections.Generic;

namespace Lunitor.DataReader.Cache
{
    interface ISensorCacheWriter
    {
        void Add(IEnumerable<SensorReading> sensorReadings);
    }
}
