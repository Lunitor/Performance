using Lunitor.Shared.Dto;
using System;
using System.Collections.Generic;

namespace Lunitor.Core.Interfaces
{
    public interface ISensorReadingRepository
    {
        IEnumerable<SensorReadingDto> Get(DateTime? from = null, DateTime? to = null, float criticalValuePercent = 1);
    }
}