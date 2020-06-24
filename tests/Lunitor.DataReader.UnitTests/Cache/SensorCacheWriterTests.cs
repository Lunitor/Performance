using Lunitor.DataReader.Cache;
using Lunitor.HardwareMonitorAPI.Models;
using Lunitor.Shared.Dto;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using Xunit;

namespace Lunitor.DataReader.UnitTests.Cache
{
    public class SensorCacheWriterTests
    {
        SensorCacheWriter _sensorCacheWriter;
        Mock<IDatabase> _databaseMock;

        public SensorCacheWriterTests()
        {
            _databaseMock = new Mock<IDatabase>();
            _sensorCacheWriter = new SensorCacheWriter(_databaseMock.Object);
        }

        [Fact]
        public void AddWithNullShouldThrowArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _sensorCacheWriter.Add(null));

            var paramName = typeof(SensorCacheWriter).GetParamName(nameof(_sensorCacheWriter.Add), 0);
            Assert.Equal(paramName, exception.ParamName);
        }

        [Fact]
        public void AddWithEmptySensorReadingsShouldNeverCallListLeftPush()
        {
            var sensorReadings = new List<SensorReading>();

            _databaseMock.Verify(db => db.ListLeftPush(It.IsAny<RedisKey>(), It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()), Times.Never);
        }

        [Fact]
        public void AddWithNotEmptySensorReadingsShouldCallListLeftPush()
        {
            var sensorReadings = new List<SensorReadingDto>()
            {
                new SensorReadingDto
                {
                    Hardware = new HardwareDto(){ Name="Hardware"},
                    Sensor = new SensorDto(){ Name="Sensor"},
                    TimeStamp = DateTime.Now,
                    Value = 1
                }
            };

            _sensorCacheWriter.Add(sensorReadings);

            _databaseMock.Verify(db => db.ListLeftPush(It.IsAny<RedisKey>(), It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()), Times.AtLeastOnce);
        }

        [Theory]
        [MemberData(nameof(SensorReadingDataAndExpectedGroupNumber))]
        public void AddWithNotEmptySensorReadingsShouldCallListLeftPushTimesAsManyHarwareSensorPairsAre(IEnumerable<SensorReadingDto> sensorReadings, int expectedGroupNumber)
        {
            _sensorCacheWriter.Add(sensorReadings);

            _databaseMock.Verify(db => db.ListLeftPush(It.IsAny<RedisKey>(), It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()), Times.Exactly(expectedGroupNumber));
        }

        [Theory]
        [MemberData(nameof(SensorReadingDataAndExpectedListKeys))]
        public void AddWithNotEmptySensorReadingsShouldCallListLeftPushTimesWithHardwareNameSensorNameKey(IEnumerable<SensorReadingDto> sensorReadings, string[] expectedListKeys)
        {
            _sensorCacheWriter.Add(sensorReadings);

            foreach (var listKey in expectedListKeys)
            {
                _databaseMock.Verify(db => db.ListLeftPush(listKey, It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()), Times.Once);
            }
        }

        [Theory]
        [MemberData(nameof(SensorReadingDataWithSpecialFloatValues))]
        public void AddWithSensorReadingDataWithSpecialFloatValuesShouldCallListLeftPush(IEnumerable<SensorReadingDto> sensorReadings)
        {
            _sensorCacheWriter.Add(sensorReadings);

            _databaseMock.Verify(db => db.ListLeftPush(It.IsAny<RedisKey>(), It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()));
        }


        public static IEnumerable<object[]> SensorReadingDataAndExpectedListKeys =>
            new List<object[]>
            {
                new object[] { _sensorReadingDtosWithOneGroup, new[] {"Hardware.Sensor"} },
                new object[] { _sensorReadingDtosWithTwoGroup, new[] {"Hardware.Sensor", "Hardware2.Sensor" } },
                new object[] { _sensorReadingDtosWithThreeGroup, new[] {"Hardware.Sensor", "Hardware2.Sensor", "Hardware3.Sensor2" } }
            };

        public static IEnumerable<object[]> SensorReadingDataAndExpectedGroupNumber => 
            new List<object[]>
            {
                new object[] { _sensorReadingDtosWithOneGroup, 1 },
                new object[] { _sensorReadingDtosWithTwoGroup, 2 },
                new object[] { _sensorReadingDtosWithThreeGroup, 3 },
            };

        public static IEnumerable<object[]> SensorReadingDataWithSpecialFloatValues =>
            new List<object[]>
            {
                        new object[] { _sensorReadingDtosWithOneGroup}
            };

        private static readonly IEnumerable<SensorReadingDto> _sensorReadingDtosWithSpecialFloatValues = new List<SensorReadingDto>() {
                                new SensorReadingDto
                                {
                                    Hardware = new HardwareDto(){ Name="Hardware"},
                                    Sensor = new SensorDto(){ Name="Sensor"},
                                    TimeStamp = DateTime.Now,
                                    Value = float.NaN
                                },
                                new SensorReadingDto
                                {
                                    Hardware = new HardwareDto(){ Name="Hardware"},
                                    Sensor = new SensorDto(){ Name="Sensor"},
                                    TimeStamp = DateTime.Now,
                                    Value = float.NegativeInfinity
                                },
                                new SensorReadingDto
                                {
                                    Hardware = new HardwareDto(){ Name="Hardware"},
                                    Sensor = new SensorDto(){ Name="Sensor"},
                                    TimeStamp = DateTime.Now,
                                    Value = float.PositiveInfinity
                                }};

        private static readonly IEnumerable<SensorReadingDto> _sensorReadingDtosWithOneGroup = new List<SensorReadingDto>() {
                                new SensorReadingDto
                                {
                                    Hardware = new HardwareDto(){ Name="Hardware"},
                                    Sensor = new SensorDto(){ Name="Sensor"},
                                    TimeStamp = DateTime.Now,
                                    Value = 1
                                }};

        private static readonly IEnumerable<SensorReadingDto> _sensorReadingDtosWithTwoGroup = new List<SensorReadingDto>() {
                                    new SensorReadingDto
                                    {
                                        Hardware = new HardwareDto(){ Name="Hardware"},
                                        Sensor = new SensorDto(){ Name="Sensor"},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReadingDto
                                    {
                                        Hardware = new HardwareDto(){ Name="Hardware2"},
                                        Sensor = new SensorDto(){ Name="Sensor"},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReadingDto
                                    {
                                        Hardware = new HardwareDto(){ Name="Hardware2"},
                                        Sensor = new SensorDto(){ Name="Sensor"},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    } };

        private static readonly IEnumerable<SensorReadingDto> _sensorReadingDtosWithThreeGroup = new List<SensorReadingDto>() {
                                    new SensorReadingDto
                                    {
                                        Hardware = new HardwareDto(){ Name="Hardware"},
                                        Sensor = new SensorDto(){ Name="Sensor"},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReadingDto
                                    {
                                        Hardware = new HardwareDto(){ Name="Hardware"},
                                        Sensor = new SensorDto(){ Name="Sensor"},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReadingDto
                                    {
                                        Hardware = new HardwareDto(){ Name="Hardware2"},
                                        Sensor = new SensorDto(){ Name="Sensor"},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReadingDto
                                    {
                                        Hardware = new HardwareDto(){ Name="Hardware2"},
                                        Sensor = new SensorDto(){ Name="Sensor"},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReadingDto
                                    {
                                        Hardware = new HardwareDto(){ Name="Hardware3"},
                                        Sensor = new SensorDto(){ Name="Sensor2"},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    } };
    }
}
