using Lunitor.DataReader.Cache;
using Lunitor.HardwareMonitorAPI.Models;
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

            _sensorCacheWriter.Add(sensorReadings);

            _databaseMock.Verify(db => db.ListLeftPush(It.IsAny<RedisKey>(), It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()), Times.AtLeastOnce);
        }

        [Theory]
        [MemberData(nameof(SensorReadingDataAndExpectedGroupNumber))]
        public void AddWithNotEmptySensorReadingsShouldCallListLeftPushTimesAsManyHarwareSensorPairsAre(IEnumerable<SensorReading> sensorReadings, int expectedGroupNumber)
        {
            _sensorCacheWriter.Add(sensorReadings);

            _databaseMock.Verify(db => db.ListLeftPush(It.IsAny<RedisKey>(), It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()), Times.Exactly(expectedGroupNumber));
        }

        [Theory]
        [MemberData(nameof(SensorReadingDataAndExpectedListKeys))]
        public void AddWithNotEmptySensorReadingsShouldCallListLeftPushTimesWithHardwareNameSensorNameKey(IEnumerable<SensorReading> sensorReadings, string[] expectedListKeys)
        {
            _sensorCacheWriter.Add(sensorReadings);

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
