using Lunitor.HardwareMonitorAPI.Models;
using System.Collections.Generic;

namespace Lunitor.Api.Cache
{
    interface ISensorReadingCache
    {
        IEnumerable<SensorReading> GetAll();
    }
}
