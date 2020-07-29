using Lunitor.Core.Models;
using Lunitor.Infrastructure.Cache;
using Lunitor.Shared;
using Lunitor.Shared.Dto;
using Microsoft.Extensions.Configuration;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Lunitor.Infrastructure.UnitTests.Cache
{
    public class SensorCacheMutatorTests
    {
        private SensorCacheMutator _sensorCacheMutator;

        private Mock<IDatabase> _databaseMock;
        private Mock<IServer> _serverMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IConfigurationSection> _configurationSectionMock;

        private const int ExpirationTimeInMinutes = 5;

        public SensorCacheMutatorTests()
        {
            _databaseMock = new Mock<IDatabase>();

            _serverMock = new Mock<IServer>();

            _configurationSectionMock = new Mock<IConfigurationSection>();
            _configurationSectionMock.Setup(sc => sc.Value)
                .Returns(ExpirationTimeInMinutes.ToString());

            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(conf => conf.GetSection(ConfigurationConstants.ExpirationTimeKey))
                .Returns(_configurationSectionMock.Object);

            _sensorCacheMutator = new SensorCacheMutator(_databaseMock.Object, _serverMock.Object, _configurationMock.Object);
        }

        [Fact]
        public void AddWithNullShouldThrowArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _sensorCacheMutator.Add(null));

            var paramName = typeof(SensorCacheMutator).GetParamName(nameof(_sensorCacheMutator.Add), 0);
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
            var testHardware = new Hardware { Name = "Hardware" };
            var sensorReadings = new List<SensorReading>()
            {
                new SensorReading
                {
                    Hardware = testHardware,
                    Sensor = new Sensor(){ Name="Sensor", Hardware = testHardware},
                    TimeStamp = DateTime.Now,
                    Value = 1
                }
            };

            _sensorCacheMutator.Add(sensorReadings);

            _databaseMock.Verify(db => db.ListLeftPush(It.IsAny<RedisKey>(), It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()), Times.AtLeastOnce);
        }

        [Theory]
        [MemberData(nameof(SensorReadingDataAndExpectedGroupNumber))]
        public void AddWithNotEmptySensorReadingsShouldCallListLeftPushTimesAsManyHarwareSensorPairsAre(IEnumerable<SensorReading> sensorReadings, int expectedGroupNumber)
        {
            _sensorCacheMutator.Add(sensorReadings);

            _databaseMock.Verify(db => db.ListLeftPush(It.IsAny<RedisKey>(), It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()), Times.Exactly(expectedGroupNumber));
        }

        [Theory]
        [MemberData(nameof(SensorReadingDataAndExpectedListKeys))]
        public void AddWithNotEmptySensorReadingsShouldCallListLeftPushTimesWithHardwareNameSensorNameKey(IEnumerable<SensorReading> sensorReadings, string[] expectedListKeys)
        {
            _sensorCacheMutator.Add(sensorReadings);

            foreach (var listKey in expectedListKeys)
            {
                _databaseMock.Verify(db => db.ListLeftPush(listKey, It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()), Times.Once);
            }
        }

        [Theory]
        [MemberData(nameof(SensorReadingDataWithSpecialFloatValues))]
        public void AddWithSensorReadingDataWithSpecialFloatValuesShouldCallListLeftPush(IEnumerable<SensorReading> sensorReadings)
        {
            _sensorCacheMutator.Add(sensorReadings);

            _databaseMock.Verify(db => db.ListLeftPush(It.IsAny<RedisKey>(), It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()));
        }

        [Fact]
        public void CleanDeletesReadingsFromCacheThatsOlderThanGivenExpirationTime()
        {
            // Arrange
            var cache = new Dictionary<string, List<SensorReadingDto>>();
            FillCache(cache);

            // setup mocks
            var serializerOptions = new JsonSerializerOptions();
            serializerOptions.Converters.Add(new FloatStringConverter());

            _databaseMock.Setup(db => db.ListLeftPush(It.IsAny<RedisKey>(), It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()))
                .Callback((RedisKey key, RedisValue[] values, CommandFlags commandFlag) =>
                {
                    foreach (var value in values)
                    {
                        if (!cache.ContainsKey(key))
                            cache.Add(key, new List<SensorReadingDto>());

                        cache[key].Add(JsonSerializer.Deserialize<SensorReadingDto>(value, serializerOptions));
                    }
                });

            _databaseMock.Setup(db => db.ListRange(It.IsAny<RedisKey>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CommandFlags>()))
                .Returns((RedisKey key, long start, long end, CommandFlags commandFlag) =>
                {
                    return cache[key].Select(sr => JsonSerializer.Serialize(sr, serializerOptions))
                        .ToArray()
                        .ToRedisValueArray();
                });

            _databaseMock.Setup(db => db.ListRemove(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<long>(), It.IsAny<CommandFlags>()))
                .Callback((RedisKey key, RedisValue value, long count, CommandFlags commandFlag) =>
                {
                    var sensorReading = JsonSerializer.Deserialize<SensorReading>(value, serializerOptions);
                    var index = cache[key].FindIndex(sr => sensorReading.TimeStamp == sr.TimeStamp
                        && sensorReading.Hardware.Name == sr.Hardware.Name
                        && sensorReading.Sensor.Name == sr.Sensor.Name);

                    cache[key].RemoveAt(index);
                });

            _serverMock.Setup(s => s.Keys(It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
                .Returns(SensorReadingCacheKeys.Select(k => new RedisKey(k)));

            // Act
            var runTimeStamp = DateTime.Now;
            _sensorCacheMutator.Clean();

            // Assert
            // verify delete called with readings older then configured expiration time
            foreach (RedisKey key in ExpiredSensorReadingCacheKeys)
            {
                _databaseMock.Verify(db => db.ListRemove(key,
                    It.Is<RedisValue>(v => v == JsonSerializer.Serialize(ExpiredSensorReadings.First(esr => $"{esr.Hardware.Name}.{esr.Sensor.Name}" == key), serializerOptions)),
                    It.IsAny<long>(),
                    It.IsAny<CommandFlags>()),
                    Times.Once);
            }

            // verify only not expired readings are in the cache
            foreach (var cacheList in cache)
            {
                Assert.All(cacheList.Value, sr => Assert.True(sr.TimeStamp >= runTimeStamp.AddMinutes(-ExpirationTimeInMinutes)));
            }

        }

        private static readonly Hardware TestHardware1 = new Hardware { Name = "Hardware" };
        private static readonly Hardware TestHardware2 = new Hardware { Name = "Hardware2" };
        private static readonly Hardware TestHardware3 = new Hardware { Name = "Hardware3" };

        #region Write Test Data
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

        public static IEnumerable<object[]> SensorReadingDataWithSpecialFloatValues =>
            new List<object[]>
            {
                        new object[] { _sensorReadingsWithOneGroup}
            };

        private static readonly IEnumerable<SensorReading> _sensorReadingsWithSpecialFloatValues = new List<SensorReading>() {
                                new SensorReading
                                {
                                    Hardware = TestHardware1,
                                    Sensor = new Sensor(){ Name="Sensor", Hardware=TestHardware1},
                                    TimeStamp = DateTime.Now,
                                    Value = float.NaN
                                },
                                new SensorReading
                                {
                                    Hardware = TestHardware1,
                                    Sensor = new Sensor(){ Name="Sensor", Hardware=TestHardware1},
                                    TimeStamp = DateTime.Now,
                                    Value = float.NegativeInfinity
                                },
                                new SensorReading
                                {
                                    Hardware = TestHardware1,
                                    Sensor = new Sensor(){ Name="Sensor", Hardware=TestHardware1},
                                    TimeStamp = DateTime.Now,
                                    Value = float.PositiveInfinity
                                }};

        private static readonly IEnumerable<SensorReading> _sensorReadingsWithOneGroup = new List<SensorReading>() {
                                new SensorReading
                                {
                                    Hardware = TestHardware1,
                                    Sensor = new Sensor(){ Name="Sensor", Hardware=TestHardware1},
                                    TimeStamp = DateTime.Now,
                                    Value = 1
                                }};

        private static readonly IEnumerable<SensorReading> _sensorReadingsWithTwoGroup = new List<SensorReading>() {
                                    new SensorReading
                                    {
                                        Hardware = TestHardware1,
                                        Sensor = new Sensor(){ Name="Sensor", Hardware=TestHardware1},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReading
                                    {
                                        Hardware = TestHardware2,
                                        Sensor = new Sensor(){ Name="Sensor", Hardware=TestHardware2},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReading
                                    {
                                        Hardware = TestHardware2,
                                        Sensor = new Sensor(){ Name="Sensor", Hardware=TestHardware2},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    } };

        private static readonly IEnumerable<SensorReading> _sensorReadingsWithThreeGroup = new List<SensorReading>() {
                                    new SensorReading
                                    {
                                        Hardware = TestHardware1,
                                        Sensor = new Sensor(){ Name="Sensor", Hardware=TestHardware1},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReading
                                    {
                                        Hardware = TestHardware1,
                                        Sensor = new Sensor(){ Name="Sensor", Hardware=TestHardware1},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReading
                                    {
                                        Hardware = TestHardware2,
                                        Sensor = new Sensor(){ Name="Sensor", Hardware=TestHardware2},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReading
                                    {
                                        Hardware = TestHardware2,
                                        Sensor = new Sensor(){ Name="Sensor", Hardware=TestHardware2},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    },
                                    new SensorReading
                                    {
                                        Hardware = TestHardware3,
                                        Sensor = new Sensor(){ Name="Sensor2", Hardware=TestHardware3},
                                        TimeStamp = DateTime.Now,
                                        Value = 1
                                    } };
        #endregion

        #region Clean Test Data
        private static List<SensorReadingDto> SensorReadings
        {
            get
            {
                var sensorReadings = new List<SensorReadingDto>();
                sensorReadings.AddRange(ExpiredSensorReadings);
                sensorReadings.AddRange(NotExpiredSensorReadings);

                return sensorReadings;
            }
        }

        private static List<string> SensorReadingCacheKeys => SensorReadings
            .Select(sr => $"{sr.Hardware.Name}.{sr.Sensor.Name}")
            .Distinct()
            .ToList();

        private static List<SensorReadingDto> ExpiredSensorReadings => new List<SensorReadingDto> {
                new SensorReadingDto
                {
                    Hardware = new HardwareDto(){ Name="Hardware"},
                    Sensor = new SensorDto(){ Name="Sensor"},
                    TimeStamp = new DateTime(2020, 7, 6, 1, 0, 0),
                    Value = 1
                },
                new SensorReadingDto
                {
                    Hardware = new HardwareDto(){ Name="Hardware"},
                    Sensor = new SensorDto(){ Name="Sensor2"},
                    TimeStamp = new DateTime(2020, 7, 6, 2, 0, 0),
                    Value = 1
                },
            };

        private static List<string> ExpiredSensorReadingCacheKeys => ExpiredSensorReadings
            .Select(sr => $"{sr.Hardware.Name}.{sr.Sensor.Name}")
            .Distinct()
            .ToList();

        private static List<SensorReadingDto> NotExpiredSensorReadings => new List<SensorReadingDto> {
                new SensorReadingDto
                {
                    Hardware = new HardwareDto(){ Name="Hardware"},
                    Sensor = new SensorDto(){ Name="Sensor"},
                    TimeStamp = DateTime.MaxValue,
                    Value = 1
                },
                new SensorReadingDto
                {
                    Hardware = new HardwareDto(){ Name="Hardware"},
                    Sensor = new SensorDto(){ Name="Sensor2"},
                    TimeStamp = DateTime.MaxValue.AddMinutes(-1),
                    Value = 1
                },
            };

        private void FillCache(Dictionary<string, List<SensorReadingDto>> cache)
        {
            foreach (var key in SensorReadingCacheKeys)
            {
                if (!cache.ContainsKey(key))
                    cache.Add(key, new List<SensorReadingDto>());

                cache[key].AddRange(SensorReadings.Where(sr => $"{sr.Hardware.Name}.{sr.Sensor.Name}" == key));
            }
        }
        #endregion
    }
}
