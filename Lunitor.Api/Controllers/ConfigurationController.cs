using Lunitor.Api.RequestModels;
using Lunitor.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lunitor.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        [HttpGet]
        [Route("periodicity")]
        public int GetPeriodicity()
        {
            return PeriodicReaderConfiguration.Periodicity;
        }

        [HttpPost]
        [Route("periodicity")]
        [Consumes("application/json")]
        public ActionResult SetPeriodicity([FromBody]SetPeriodicityRequest request)
        {
            if (request.Periodicity <= 0)
                return BadRequest($"Periodicty can not be 0 or negative number");

            PeriodicReaderConfiguration.Periodicity = request.Periodicity;
            return Ok($"Periodicty set to {request.Periodicity}s");
        }
    }
}
