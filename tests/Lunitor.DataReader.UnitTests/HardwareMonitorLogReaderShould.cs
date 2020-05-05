using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace Lunitor.DataReader.UnitTests
{
    public class HardwareMonitorLogReaderShould
    {
        private HardwareMonitorLogReader _hardwareMonitorLogReader;
        private Mock<ILogger<HardwareMonitorLogReader>> _mockLogger;

        public HardwareMonitorLogReaderShould()
        {
            _mockLogger = new Mock<ILogger<HardwareMonitorLogReader>>();
            _hardwareMonitorLogReader = new HardwareMonitorLogReader(_mockLogger.Object);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ThrowExceptionWhenReadGetsNullOrEmptyString(string fileName)
        {
            Assert.ThrowsAny<Exception>(() => _hardwareMonitorLogReader.Read(fileName));
        }
    }
}
