using Lunitor.HardwareMonitorAPI.Models;
using System.Collections.Generic;

namespace Lunitor.Api.Cache
{
    interface ISensorCacheReader
    {
        IEnumerable<SensorReading> GetAll();
    }
}
