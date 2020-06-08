using Lunitor.DataReader.Cache;
using Lunitor.HardwareMonitorAPI.Models;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using Xunit;

namespace Lunitor.DataReader.UnitTests.Cache
{
    public class SensorReadingCacheTests
    {
        SensorReadingCache _sensorReadingCache;
        Mock<IDatabase> _databaseMock;

        public SensorReadingCacheTests()
        {
            _databaseMock = new Mock<IDatabase>();
            _sensorReadingCache = new SensorReadingCache(_databaseMock.Object);
        }

        [Fact]
        public void AddWithNullShouldThrowArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _sensorReadingCache.Add(null));

            var paramName = typeof(SensorReadingCache).GetParamName(nameof(_sensorReadingCache.Add), 0);
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
            var sensorReadings = new List<SensorReading>()
            {
                new SensorReading
                {
                    Hardware = new Hardware(){ Name="Hardware"},
                    Sensor = new Sensor(){ Name="Sensor"},
                    TimeStamp = DateTime.Now,
                    Value = 1
                }
            };

            _sensorReadingCache.Add(sensorReadings);

            _databaseMock.Verify(db => db.ListLeftPush(It.IsAny<RedisKey>(), It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()), Times.AtLeastOnce);
        }

        [Theory]
        [MemberData(nameof(SensorReadingDataAndExpectedGroupNumber))]
        public void AddWithNotEmptySensorReadingsShouldCallListLeftPushTimesAsManyHarwareSensorPairsAre(IEnumerable<SensorReading> sensorReadings, int expectedGroupNumber)
        {
            _sensorReadingCache.Add(sensorReadings);

            _databaseMock.Verify(db => db.ListLeftPush(It.IsAny<RedisKey>(), It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()), Times.Exactly(expectedGroupNumber));
        }

        [Theory]
        [MemberData(nameof(SensorReadingDataAndExpectedListKeys))]
        public void AddWithNotEmptySensorReadingsShouldCallListLeftPushTimesWithHardwareNameSensorNameKey(IEnumerable<SensorReading> sensorReadings, string[] expectedListKeys)
        {
            _sensorReadingCache.Add(sensorReadings);

            foreach (var listKey in expectedListKeys)
            {
                _databaseMock.Verify(db => db.ListLeftPush(listKey, It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()), Times.Once);
            }
        }


        public static IEnumerable<object[]> SensorReadingDataAndExpectedListKeys =>
            new List<object[]>
            {
                new object[] { _sensorReadingsWithOneGroup, new[] {"Hardware.Sensor"} },
                new object[] { _sensorReadingsWithTwoGroup, new[] {"Hardware.Sensor", "Hardware2.Sensor" } },
                new object[] { _sensorReadingsWithThreeGroup, new[] {"Hardware.Sensor", "Hardware2.Sensor", "Hardware3.Sensor2" } }
            };

        public static IEnumerable<object[]> SensorReadingDataAndExpectedGroupNumber => 
            new List<object[]>
            {
                new object[] { _sensorReadingsWithOneGroup, 1 },
                new object[] { _sensorReadingsWithTwoGroup, 2 },
                new object[] { _sensorReadingsWithThreeGroup, 3 },
            };

        private static readonly IEnumerable<SensorReading> _sensorReadingsWithOneGroup = new List<SensorReading>() {
                                new SensorReading
                                {
                                    Hardware = new Hardware(){ Name="Hardware"},
                                    Sensor = new Sensor(){ Name="Sensor"},
                                    TimeStamp = DateTime.Now,
                                    Value = 1
                                }};

        private static readonly IEnumerable<SensorReading> _sensorReadingsWithTwoGroup = new List<SensorReading>() {
                                    new SensorReading
                                    {
                                        Hardware = new Hardware(){ Name="Hardware"},
                                        Sensor = new Sensor(){ Name="Sensor"},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReading
                                    {
                                        Hardware = new Hardware(){ Name="Hardware2"},
                                        Sensor = new Sensor(){ Name="Sensor"},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReading
                                    {
                                        Hardware = new Hardware(){ Name="Hardware2"},
                                        Sensor = new Sensor(){ Name="Sensor"},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    } };

        private static readonly IEnumerable<SensorReading> _sensorReadingsWithThreeGroup = new List<SensorReading>() {
                                    new SensorReading
                                    {
                                        Hardware = new Hardware(){ Name="Hardware"},
                                        Sensor = new Sensor(){ Name="Sensor"},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReading
                                    {
                                        Hardware = new Hardware(){ Name="Hardware"},
                                        Sensor = new Sensor(){ Name="Sensor"},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReading
                                    {
                                        Hardware = new Hardware(){ Name="Hardware2"},
                                        Sensor = new Sensor(){ Name="Sensor"},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReading
                                    {
                                        Hardware = new Hardware(){ Name="Hardware2"},
                                        Sensor = new Sensor(){ Name="Sensor"},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReading
                                    {
                                        Hardware = new Hardware(){ Name="Hardware3"},
                                        Sensor = new Sensor(){ Name="Sensor2"},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    } };
    }
}
