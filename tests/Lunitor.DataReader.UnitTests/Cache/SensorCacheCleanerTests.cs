using Lunitor.DataReader.Cache;
using Lunitor.DataReader.Persistance.Entities;
using Lunitor.Shared.Dto;
using Lunitor.Shared.Json;
using Microsoft.Extensions.Configuration;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Xunit;

namespace Lunitor.DataReader.UnitTests.Cache
{
    public class SensorCacheCleanerTests
    {
        private SensorCacheCleaner _sensorCacheCleaner;

        private Mock<IDatabase> _databaseMock;
        private Mock<IServer> _serverMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IConfigurationSection> _configurationSectionMock;

        private const int ExpirationTimeInMinutes = 5;

        public SensorCacheCleanerTests()
        {
            _databaseMock = new Mock<IDatabase>();

            _serverMock = new Mock<IServer>();

            _configurationSectionMock = new Mock<IConfigurationSection>();
            _configurationSectionMock.Setup(sc => sc.Value)
                .Returns(ExpirationTimeInMinutes.ToString());

            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(conf => conf.GetSection("CacheWriter:ExpirationTime"))
                .Returns(_configurationSectionMock.Object);

            _sensorCacheCleaner = new SensorCacheCleaner(_databaseMock.Object, _serverMock.Object, _configurationMock.Object);
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
                    var sensorReading = JsonSerializer.Deserialize<SensorReadingDto>(value, serializerOptions);
                    var index = cache[key].FindIndex(sr => sensorReading.TimeStamp == sr.TimeStamp
                        && sensorReading.Hardware.Name == sr.Hardware.Name
                        && sensorReading.Sensor.Name == sr.Sensor.Name);

                    cache[key].RemoveAt(index);
                });

            _serverMock.Setup(s => s.Keys(It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
                .Returns(SensorReadingCacheKeys.Select(k => new RedisKey(k)));

            // Act
            var runTimeStamp = DateTime.Now;
            _sensorCacheCleaner.Clean();

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
    }
}
