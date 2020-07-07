using Lunitor.Shared.Dto;
using System;
using System.Collections.Generic;

namespace Lunitor.Api.Services
{
    public interface ISensorReadingService
    {
        IEnumerable<SensorReadingDto> Get(DateTime? from = null, DateTime? to = null, float criticalValuePercent = 1);
    }
}