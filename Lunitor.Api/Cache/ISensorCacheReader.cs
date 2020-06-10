using Lunitor.HardwareMonitorAPI.Models;
using System.Collections.Generic;

namespace Lunitor.Api.Cache
{
    public interface ISensorCacheReader
    {
        IEnumerable<SensorReading> GetAll();
    }
}
