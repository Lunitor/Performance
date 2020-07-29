using Lunitor.Core.Models;
using System.Collections.Generic;

namespace Lunitor.Core.Interfaces.Cache
{
    public interface ISensorCacheMutator
    {
        void Clean();
        void Add(IEnumerable<SensorReading> sensorReadings);
    }
}
