using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace Lunitor.DataReader.UnitTests
{
    public class HardwareInfoFileReaderShould
    {
        private HardwareInfoFileReader _hardwareInfoFileReader;
        private Mock<ILogger<HardwareInfoFileReader>> _mockLogger;

        public HardwareInfoFileReaderShould()
        {
            _mockLogger = new Mock<ILogger<HardwareInfoFileReader>>();
            _hardwareInfoFileReader = new HardwareInfoFileReader(_mockLogger.Object);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ThrowExceptionWhenReadGetsNullOrEmptyString(string fileName)
        {
            Assert.ThrowsAny<Exception>(() => _hardwareInfoFileReader.Read(fileName));
        }
    }
}
