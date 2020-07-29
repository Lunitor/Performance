using Lunitor.Shared.Dto;
using System.Collections.Generic;

namespace Lunitor.Core.Interfaces.Cache
{
    public interface ISensorCacheReader
    {
        IEnumerable<SensorReadingDto> GetAll();
    }
}
