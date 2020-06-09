using Lunitor.Api.Cache;
using Lunitor.HardwareMonitorAPI.Models;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Xunit;

namespace Lunitor.Api.UnitTests.Cache
{
    public class SensorReadingCacheTests
    {
        private SensorReadingCache _sensorReadingCache;
        private Mock<IDatabase> _databaseMock;
        private Mock<IServer> _server;

        public SensorReadingCacheTests()
        {
            _databaseMock = new Mock<IDatabase>();
            _databaseMock.Setup(db => db.ListRange(It.IsAny<RedisKey>(), 0, -1, It.IsAny<CommandFlags>()))
                .Returns(SensorReadingsTestData_Serialized);

            _server = new Mock<IServer>();
            _server.Setup(s => s.Keys(It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
                .Returns(new List<RedisKey>() { "key" });

            _sensorReadingCache = new SensorReadingCache(_databaseMock.Object, _server.Object);
        }

        [Fact]
        public void GetAllShouldReturnEmptyIEnumerableWhenCacheEmpty()
        {
            _databaseMock.Setup(db => db.ListRange(It.IsAny<RedisKey>(), 0, -1, It.IsAny<CommandFlags>()))
                .Returns(new RedisValue[0]);

            var result = _sensorReadingCache.GetAll();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetAllShouldReturnAllSensorReadingsFromCache()
        {
            var result = _sensorReadingCache.GetAll();

            Assert.NotEmpty(result);
            Assert.Equal(SensorReadingsTestData_Serialized.Count(), result.Count());
        }

        [Fact]
        public void GetAllShouldReturnCorrectlyDeserializedSensorReadings()
        {
            var result = _sensorReadingCache.GetAll();

            Assert.NotEmpty(result);
            foreach (var sensorReading in result)
            {
                Assert.NotNull(sensorReading.Hardware);
                Assert.NotNull(sensorReading.Sensor);

                var expectedSensorReading = SensorReadingsTestData
                    .FirstOrDefault(srtd => $"{srtd.Hardware.Name}.{srtd.Sensor.Name}" == $"{sensorReading.Hardware.Name}.{sensorReading.Sensor.Name}"
                        && DateTime.Equals(srtd.TimeStamp, sensorReading.TimeStamp));

                Assert.NotNull(expectedSensorReading);
                Assert.Equal(expectedSensorReading.Value, sensorReading.Value);
            }
        }

        public static IEnumerable<SensorReading> SensorReadingsTestData => new List<SensorReading>() {
                                    new SensorReading
                                    {
                                        Hardware = new Hardware(){ Name="Hardware"},
                                        Sensor = new Sensor(){ Name="Sensor"},
                                        TimeStamp = new DateTime(2020, 6, 9, 14, 54, 32),
                                        Value = 1
                                    },
                                    new SensorReading
                                    {
                                        Hardware = new Hardware(){ Name="Hardware"},
                                        Sensor = new Sensor(){ Name="Sensor"},
                                        TimeStamp = new DateTime(2020, 6, 9, 14, 58, 10),
                                        Value = 1
                                    },
                                    new SensorReading
                                    {
                                        Hardware = new Hardware(){ Name="Hardware2"},
                                        Sensor = new Sensor(){ Name="Sensor"},
                                        TimeStamp = new DateTime(2020, 6, 9, 14, 54, 32),
                                        Value = 1
                                    },
                                    new SensorReading
                                    {
                                        Hardware = new Hardware(){ Name="Hardware2"},
                                        Sensor = new Sensor(){ Name="Sensor"},
                                        TimeStamp = new DateTime(2020, 6, 9, 14, 58, 15),
                                        Value = 1
                                    },
                                    new SensorReading
                                    {
                                        Hardware = new Hardware(){ Name="Hardware3"},
                                        Sensor = new Sensor(){ Name="Sensor2"},
                                        TimeStamp = new DateTime(2020, 6, 9, 14, 54, 32),
                                        Value = 1
                                    } };

        public static RedisValue[] SensorReadingsTestData_Serialized => 
            SensorReadingsTestData
            .Select(sr => JsonSerializer.Serialize(sr))
            .ToArray()
            .ToRedisValueArray();
    }
}
