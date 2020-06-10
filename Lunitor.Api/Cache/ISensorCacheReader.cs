using Lunitor.Shared.Dto;
using System.Collections.Generic;

namespace Lunitor.Api.Cache
{
    public interface ISensorCacheReader
    {
        IEnumerable<SensorReadingDto> GetAll();
    }
}
