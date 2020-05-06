using Lunitor.DataReader.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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

        [Fact]
        public void ThrowExceptionWhenReadGetsNullLog()
        {
            Assert.ThrowsAny<Exception>(() => _hardwareMonitorLogReader.Read(null));
        }

        [Fact]
        public void SkipLinesThatHasUnknownLineType()
        {
            var result = _hardwareMonitorLogReader.Read(new []{ "123456, 342, 1244"});
            Assert.Empty(result);
        }

        [Fact]
        public void ReturnParametersThatAreIn02TypeLine()
        {
            var paramLine = HardwareInfoLogTestData.CreateParameterLine();

            var result = _hardwareMonitorLogReader.Read(new[] { paramLine });

            foreach (var paramName in HardwareInfoLogTestData.ParamNames)
                Assert.NotNull(result.Keys.FirstOrDefault(p => p.Name == paramName));
        }

        [Fact]
        public void ReturnParamtersWithMinMaxUnitofMesasureFilled()
        {
            var paramLine = HardwareInfoLogTestData.CreateParameterLine();
            var paramInfoLines = HardwareInfoLogTestData.CreateParameterInfoLines();

            var log = new List<string>();
            log.Add(paramLine);
            log.AddRange(paramInfoLines);

            var result = _hardwareMonitorLogReader.Read(log);

            for (int i = 0; i < HardwareInfoLogTestData.ParamNames.Length; i++)
            {
                var param = result.Keys.FirstOrDefault(p => p.Name == HardwareInfoLogTestData.ParamNames[i]);
                Assert.NotNull(param);
                Assert.Equal(HardwareInfoLogTestData.ParamUoMs[i], param.UnitofMeasure);
                Assert.Equal(HardwareInfoLogTestData.ParamMins[i], param.Min);
                Assert.Equal(HardwareInfoLogTestData.ParamMaxs[i], param.Max);
            }

        }

        [Fact]
        public void ReturnParamtersWithCorrectDataValues()
        {
            var result = _hardwareMonitorLogReader.Read(HardwareInfoLogTestData.FullLog);

            for (int paramIndex = 0; paramIndex < HardwareInfoLogTestData.ParamNames.Length; paramIndex++)
            {
                var param = result.Keys.FirstOrDefault(p => p.Name == HardwareInfoLogTestData.ParamNames[paramIndex]);
                Assert.NotNull(param);

                for (int rowIndex = 0; rowIndex < HardwareInfoLogTestData.Values.Count; rowIndex++)
                    Assert.Equal(HardwareInfoLogTestData.Values[rowIndex][paramIndex], result[param][rowIndex].Value);
            }
        }

        [Fact]
        public void ReturnDataWithCorrectTimeStamps()
        {
            var result = _hardwareMonitorLogReader.Read(HardwareInfoLogTestData.FullLog);

            for (int paramIndex = 0; paramIndex < HardwareInfoLogTestData.ParamNames.Length; paramIndex++)
            {
                var param = result.Keys.FirstOrDefault(p => p.Name == HardwareInfoLogTestData.ParamNames[paramIndex]);
                Assert.NotNull(param);

                for (int rowIndex = 0; rowIndex < HardwareInfoLogTestData.Values.Count; rowIndex++)
                    Assert.Equal(HardwareInfoLogTestData.TimeStamps[rowIndex], result[param][rowIndex].TimeStamp.ToString("dd-MM-yyyy HH:mm:ss"));
            }
        }

        [Fact]
        public void SetDataValueToNullWhenInLogItIsNA()
        {
            var result = _hardwareMonitorLogReader.Read(HardwareInfoLogTestData.FullLogWithNAValues);

            for (int paramIndex = 0; paramIndex < HardwareInfoLogTestData.ParamNames.Length; paramIndex++)
            {
                var param = result.Keys.FirstOrDefault(p => p.Name == HardwareInfoLogTestData.ParamNames[paramIndex]);
                Assert.NotNull(param);

                for (int rowIndex = 0; rowIndex < HardwareInfoLogTestData.Values.Count; rowIndex++)
                    Assert.Equal(HardwareInfoLogTestData.ValuesWithNA[rowIndex][paramIndex], result[param][rowIndex].Value);
            }
        }


    }
}
