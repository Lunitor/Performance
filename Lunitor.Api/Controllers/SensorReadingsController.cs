using Lunitor.Api.Cache;
using Lunitor.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Lunitor.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SensorReadingsController : ControllerBase
    {
        private readonly ISensorCacheReader _sensorCacheReader;

        public SensorReadingsController(ISensorCacheReader sensorCacheReader)
        {
            _sensorCacheReader = sensorCacheReader;
        }

        [HttpGet]
        public IEnumerable<SensorReadingDto> GetAll()
        {
            return _sensorCacheReader.GetAll();
        }
    }
}