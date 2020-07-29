using Lunitor.Core.Interfaces.Cache;
using Lunitor.Shared.Dto;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Lunitor.Infrastructure.UnitTests
{
    public class SensorReadingRepositoryTests
    {
        private SensorReadingRepository _sensorReadingRepository;

        private Mock<ISensorCacheReader> _sensorCacheMock;

        public SensorReadingRepositoryTests()
        {
            _sensorCacheMock = new Mock<ISensorCacheReader>();
            _sensorCacheMock.Setup(sc => sc.GetAll())
                .Returns(SensorReadingsTestData);

            _sensorReadingRepository = new SensorReadingRepository(_sensorCacheMock.Object);
        }

        [Fact]
        public void GetShouldNotReturnNull()
        {
            var result = _sensorReadingRepository.Get();

            Assert.NotNull(result);
        }

        [Fact]
        public void GetShouldReturnEmptyEnumerableWhenThereAreNoSensorReadings()
        {
            _sensorCacheMock.Setup(sc => sc.GetAll())
                .Returns(new List<SensorReadingDto>());

            var result = _sensorReadingRepository.Get();

            Assert.IsAssignableFrom<IEnumerable<SensorReadingDto>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetShouldReturnAllSensorReadingsWhenNoParameterPassed()
        {
            var result = _sensorReadingRepository.Get();

            Assert.Equal(SensorReadingsTestData.Count(), result.Count());
        }

        [Fact]
        public void GetShouldReturnSensorReadingsThatNewerOrEquelThanGivenFromParameterDateTime()
        {
            var from = new DateTime(2020, 6, 9, 14, 58, 15);

            var result = _sensorReadingRepository.Get(from: from);

            Assert.All(result, sensorReading => Assert.True(sensorReading.TimeStamp >= from));
            Assert.All(result, sensorReading => Assert.False(sensorReading.TimeStamp < from));
        }

        [Fact]
        public void GetShouldReturnSensorReadingsThatOlderOrEquelThanGivenToParameterDateTime()
        {
            var to = new DateTime(2020, 6, 9, 14, 58, 10);

            var result = _sensorReadingRepository.Get(to: to);

            Assert.All(result, sensorReading => Assert.True(sensorReading.TimeStamp <= to));
            Assert.All(result, sensorReading => Assert.False(sensorReading.TimeStamp > to));
        }

        [Fact]
        public void GetShouldReturnSensorReadingsThatBetweenTheGivenFromAndToParameterDateTime()
        {
            var from = new DateTime(2020, 6, 9, 14, 58, 10);
            var to = new DateTime(2020, 6, 9, 14, 58, 15);

            var result = _sensorReadingRepository.Get(from: from, to: to);

            Assert.All(result, sensorReading => Assert.True(sensorReading.TimeStamp >= from && sensorReading.TimeStamp <= to));
            Assert.All(result, sensorReading => Assert.False(sensorReading.TimeStamp < from && sensorReading.TimeStamp > to));
        }

        [Fact]
        public void GetShouldReturnSensorReadingsThatValuesIsAboveTheSensorMaxValuesGivenPercentage()
        {
            var criticalValuePercent = 0.8f;

            var result = _sensorReadingRepository.Get(criticalValuePercent: criticalValuePercent);

            Assert.All(result, sensorReading => Assert.True(sensorReading.Value >= criticalValuePercent * sensorReading.Sensor.MaxValue));
            Assert.All(result, sensorReading => Assert.False(sensorReading.Value < criticalValuePercent * sensorReading.Sensor.MaxValue));
        }

        [Fact]
        public void GetShouldReturnSensorReadingsThatValuesIsAboveTheSensorMaxValuesGivenPercentageAndFilteredWithGivenFromToDateTimes()
        {
            var from = new DateTime(2020, 6, 9, 14, 58, 10);
            var to = new DateTime(2020, 6, 9, 14, 58, 15);
            var criticalValuePercent = 0.8f;

            var resultWithFrom = _sensorReadingRepository.Get(from: from, criticalValuePercent: criticalValuePercent);
            var resultWithTo = _sensorReadingRepository.Get(to: to, criticalValuePercent: criticalValuePercent);
            var resultWithFromTo = _sensorReadingRepository.Get(from: from, to: to, criticalValuePercent: criticalValuePercent);


            Assert.All(resultWithFrom, sensorReading => Assert.True(sensorReading.TimeStamp >= from
                && sensorReading.Value >= criticalValuePercent * sensorReading.Sensor.MaxValue));
            Assert.All(resultWithFrom, sensorReading => Assert.False(sensorReading.TimeStamp < from
                || sensorReading.Value < criticalValuePercent * sensorReading.Sensor.MaxValue));

            Assert.All(resultWithTo, sensorReading => Assert.True(sensorReading.TimeStamp <= to
                && sensorReading.Value >= criticalValuePercent * sensorReading.Sensor.MaxValue));
            Assert.All(resultWithTo, sensorReading => Assert.False(sensorReading.TimeStamp > to
                || sensorReading.Value < criticalValuePercent * sensorReading.Sensor.MaxValue));

            Assert.All(resultWithFromTo, sensorReading => Assert.True(sensorReading.TimeStamp >= from 
                && sensorReading.TimeStamp <= to 
                && sensorReading.Value >= criticalValuePercent * sensorReading.Sensor.MaxValue));
            Assert.All(resultWithFromTo, sensorReading => Assert.False(sensorReading.TimeStamp < from && sensorReading.TimeStamp > to
                || sensorReading.Value < criticalValuePercent * sensorReading.Sensor.MaxValue));
        }

        public static IEnumerable<SensorReadingDto> SensorReadingsTestData
        {
            get
            {
                var hardware = new HardwareDto() { Name = "Hardware" };
                var hardware2 = new HardwareDto() { Name = "Hardware2" };
                var hardware3 = new HardwareDto() { Name = "Hardware3" };
                var sensor = new SensorDto()
                {
                    Name = "Sensor",
                    MinValue = 0,
                    MaxValue = 1.0f
                };
                var sensor2 = new SensorDto()
                {
                    Name = "Sensor2",
                    MinValue = 0,
                    MaxValue = 1.0f
                };

                return new List<SensorReadingDto>()
                {
                    new SensorReadingDto
                    {

                        Hardware = hardware,
                        Sensor = sensor,
                        TimeStamp = new DateTime(2020, 6, 9, 14, 54, 32),
                        Value = 1.0f
                    },
                    new SensorReadingDto
                    {
                        Hardware = hardware,
                        Sensor = sensor,
                        TimeStamp = new DateTime(2020, 6, 9, 14, 58, 10),
                        Value = 0.9f
                    },
                    new SensorReadingDto
                    {
                        Hardware = hardware2,
                        Sensor = sensor,
                        TimeStamp = new DateTime(2020, 6, 9, 14, 54, 32),
                        Value = 0.8f
                    },
                    new SensorReadingDto
                    {
                        Hardware = hardware2,
                        Sensor = sensor,
                        TimeStamp = new DateTime(2020, 6, 9, 14, 58, 15),
                        Value = 0f
                    },
                    new SensorReadingDto
                    {
                        Hardware = hardware3,
                        Sensor = sensor2,
                        TimeStamp = new DateTime(2020, 6, 9, 14, 54, 32),
                        Value = 1.0f
                    },
                    new SensorReadingDto
                    {
                        Hardware = hardware,
                        Sensor = sensor,
                        TimeStamp = new DateTime(2020, 6, 9, 15, 00, 00),
                        Value = float.NaN
                    },
                };
            }
        }
    }
}
