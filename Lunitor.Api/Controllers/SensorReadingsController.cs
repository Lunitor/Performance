using Lunitor.Core.Interfaces;
using Lunitor.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Lunitor.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SensorReadingsController : ControllerBase
    {
        private readonly ISensorReadingRepository _sensorReadingRepository;

        public SensorReadingsController(ISensorReadingRepository sensorReadingRepository)
        {
            _sensorReadingRepository = sensorReadingRepository;
        }

        [HttpGet]
        public IEnumerable<SensorReadingDto> GetAll()
        {
            return _sensorReadingRepository.Get();

        }
    }
}