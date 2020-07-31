using Lunitor.Api.Controllers;
using Lunitor.Api.RequestModels;
using Lunitor.Shared;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Lunitor.Api.UnitTests.Controllers
{
    public class ConfigurationControllerTests
    {
        private ConfigurationController configurationController;

        public ConfigurationControllerTests()
        {
            configurationController = new ConfigurationController();
        }

        [Fact]
        public void GetPeriodicityShouldReturnPeriodicityStoredInPeriodicReaderConfiguration()
        {
            var testPeriodicity = 5;
            PeriodicReaderConfiguration.Periodicity = testPeriodicity;

            var result = configurationController.GetPeriodicity();

            Assert.Equal(testPeriodicity, result);
        }

        [Fact]
        public void SetPeriodicityShouldModifyPeriodicReaderConfigurationsPeriodicityAndReturnOkResult()
        {
            var testPeriodicity = 10;

            PeriodicReaderConfiguration.Periodicity = 5;

            var request = new SetPeriodicityRequest
            {
                Periodicity = testPeriodicity
            };

            var result = configurationController.SetPeriodicity(request);

            Assert.IsAssignableFrom<OkObjectResult>(result);
            Assert.Equal(testPeriodicity, PeriodicReaderConfiguration.Periodicity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void SetPeriodicityShouldReturnBadRequestWhenPeriodicityInRequestZeroOrNegative(int periodicty)
        {
            PeriodicReaderConfiguration.Periodicity = 5;

            var request = new SetPeriodicityRequest
            {
                Periodicity = periodicty
            };

            var result = configurationController.SetPeriodicity(request);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }
    }
}
