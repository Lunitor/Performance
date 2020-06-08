using Lunitor.HardwareMonitorAPI.Models;
using System.Collections.Generic;

namespace Lunitor.DataReader.Cache
{
    interface ISensorReadingCache
    {
        void Add(IEnumerable<SensorReading> sensorReadings);
    }
}
