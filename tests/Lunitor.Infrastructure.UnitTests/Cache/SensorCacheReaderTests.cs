using Lunitor.Core.Models;
using Lunitor.Infrastructure.Cache;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Lunitor.Infrastructure.UnitTests.Cache
{
    public class SensorCacheReaderTests
    {
        private SensorCacheReader _sensorCacheReader;
        private Mock<IDatabase> _databaseMock;
        private Mock<IServer> _server;

        public SensorCacheReaderTests()
        {
            _databaseMock = new Mock<IDatabase>();
            _databaseMock.Setup(db => db.ListRange(It.IsAny<RedisKey>(), 0, -1, It.IsAny<CommandFlags>()))
                .Returns(SensorReadingsTestData_Serialized);

            _server = new Mock<IServer>();
            _server.Setup(s => s.Keys(It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
                .Returns(new List<RedisKey>() { "key" });

            _sensorCacheReader = new SensorCacheReader(_databaseMock.Object, _server.Object);
        }

        [Fact]
        public void GetAllShouldReturnEmptyIEnumerableWhenCacheEmpty()
        {
            _databaseMock.Setup(db => db.ListRange(It.IsAny<RedisKey>(), 0, -1, It.IsAny<CommandFlags>()))
                .Returns(new RedisValue[0]);

            var result = _sensorCacheReader.GetAll();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetAllShouldReturnAllSensorReadingsFromCache()
        {
            var result = _sensorCacheReader.GetAll();

            Assert.NotEmpty(result);
            Assert.Equal(SensorReadingsTestData_Serialized.Count(), result.Count());
        }

        [Fact]
        public void GetAllShouldReturnCorrectlyDeserializedSensorReadings()
        {
            var result = _sensorCacheReader.GetAll();

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

        [Fact]
        public void GetAllShouldReturnCorrectlyDeserializedSensorReadingsThatHaveSpecialFloatValues()
        {
            _databaseMock.Setup(db => db.ListRange(It.IsAny<RedisKey>(), 0, -1, It.IsAny<CommandFlags>()))
                .Returns(SensorReadingsTestDataWithSpecialFloatValues_Serialized);

            var result = _sensorCacheReader.GetAll();

            Assert.NotEmpty(result);
            foreach (var sensorReading in result)
            {
                Assert.NotNull(sensorReading.Hardware);
                Assert.NotNull(sensorReading.Sensor);

                var expectedSensorReading = SensorReadingsWithSpecialFloatValuesTestData
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

        public static IEnumerable<SensorReading> SensorReadingsWithSpecialFloatValuesTestData => new List<SensorReading>() {
                                    new SensorReading
                                    {
                                        Hardware = new Hardware(){ Name="Hardware"},
                                        Sensor = new Sensor(){ Name="Sensor", MinValue=float.NegativeInfinity},
                                        TimeStamp = new DateTime(2020, 6, 9, 14, 54, 32),
                                        Value = float.NegativeInfinity
                                    },
                                    new SensorReading
                                    {
                                        Hardware = new Hardware(){ Name="Hardware"},
                                        Sensor = new Sensor(){ Name="Sensor"},
                                        TimeStamp = new DateTime(2020, 6, 9, 14, 58, 10),
                                        Value = float.NaN
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
                                        Sensor = new Sensor(){ Name="Sensor", MaxValue=float.PositiveInfinity},
                                        TimeStamp = new DateTime(2020, 6, 9, 14, 58, 15),
                                        Value = float.NaN
                                    },
                                    new SensorReading
                                    {
                                        Hardware = new Hardware(){ Name="Hardware3"},
                                        Sensor = new Sensor(){ Name="Sensor2"},
                                        TimeStamp = new DateTime(2020, 6, 9, 14, 54, 32),
                                        Value = float.PositiveInfinity
                                    } };

        //public static RedisValue[] SensorReadingsTestData_Serialized => 
        //    SensorReadingsTestData
        //    .Select(sr => JsonSerializer.Serialize(sr))
        //    .ToArray()
        //    .ToRedisValueArray();
        public static RedisValue[] SensorReadingsTestData_Serialized => new RedisValue[] {
            "{\"TimeStamp\":\"2020-06-09T14:54:32\",\"Hardware\":{\"Type\":null,\"Name\":\"Hardware\"},\"Sensor\":{\"Hardware\":null,\"Type\":null,\"Name\":\"Sensor\",\"MinValue\":null,\"MaxValue\":null},\"Value\":1}",
            "{\"TimeStamp\":\"2020-06-09T14:58:10\",\"Hardware\":{\"Type\":null,\"Name\":\"Hardware\"},\"Sensor\":{\"Hardware\":null,\"Type\":null,\"Name\":\"Sensor\",\"MinValue\":null,\"MaxValue\":null},\"Value\":1}",
            "{\"TimeStamp\":\"2020-06-09T14:54:32\",\"Hardware\":{\"Type\":null,\"Name\":\"Hardware2\"},\"Sensor\":{\"Hardware\":null,\"Type\":null,\"Name\":\"Sensor\",\"MinValue\":null,\"MaxValue\":null},\"Value\":1}",
            "{\"TimeStamp\":\"2020-06-09T14:58:15\",\"Hardware\":{\"Type\":null,\"Name\":\"Hardware2\"},\"Sensor\":{\"Hardware\":null,\"Type\":null,\"Name\":\"Sensor\",\"MinValue\":null,\"MaxValue\":null},\"Value\":1}",
            "{\"TimeStamp\":\"2020-06-09T14:54:32\",\"Hardware\":{\"Type\":null,\"Name\":\"Hardware3\"},\"Sensor\":{\"Hardware\":null,\"Type\":null,\"Name\":\"Sensor2\",\"MinValue\":null,\"MaxValue\":null},\"Value\":1}"
        };

        public static RedisValue[] SensorReadingsTestDataWithSpecialFloatValues_Serialized => new RedisValue[] {
            "{\"TimeStamp\":\"2020-06-09T14:54:32\",\"Hardware\":{\"Type\":null,\"Name\":\"Hardware\"},\"Sensor\":{\"Hardware\":null,\"Type\":null,\"Name\":\"Sensor\",\"MinValue\":\"-infinity\",\"MaxValue\":null},\"Value\":\"-∞\"}",
            "{\"TimeStamp\":\"2020-06-09T14:58:10\",\"Hardware\":{\"Type\":null,\"Name\":\"Hardware\"},\"Sensor\":{\"Hardware\":null,\"Type\":null,\"Name\":\"Sensor\",\"MinValue\":null,\"MaxValue\":null},\"Value\":\"NaN\"}",
            "{\"TimeStamp\":\"2020-06-09T14:54:32\",\"Hardware\":{\"Type\":null,\"Name\":\"Hardware2\"},\"Sensor\":{\"Hardware\":null,\"Type\":null,\"Name\":\"Sensor\",\"MinValue\":null,\"MaxValue\":null},\"Value\":1}",
            "{\"TimeStamp\":\"2020-06-09T14:58:15\",\"Hardware\":{\"Type\":null,\"Name\":\"Hardware2\"},\"Sensor\":{\"Hardware\":null,\"Type\":null,\"Name\":\"Sensor\",\"MinValue\":null,\"MaxValue\":\"∞\"},\"Value\":\"NaN\"}",
            "{\"TimeStamp\":\"2020-06-09T14:54:32\",\"Hardware\":{\"Type\":null,\"Name\":\"Hardware3\"},\"Sensor\":{\"Hardware\":null,\"Type\":null,\"Name\":\"Sensor2\",\"MinValue\":null,\"MaxValue\":null},\"Value\":\"infinity\"}"
        };
    }
}
