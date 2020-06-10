using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lunitor.Api.Cache;
using Lunitor.HardwareMonitorAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IEnumerable<SensorReading> GetAll()
        {
            return _sensorCacheReader.GetAll();
        }
    }
}