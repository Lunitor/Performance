﻿using Lunitor.Api.Controllers;
using Lunitor.Core.Interfaces;
using Lunitor.Shared.Dto;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Lunitor.Api.UnitTests.Controllers
{
    public class SensorReadingsControllerTests
    {
        private SensorReadingsController _sensorReadingsController;
        private Mock<ISensorReadingRepository> _sensorReadingRepositoryMock;

        public SensorReadingsControllerTests()
        {
            _sensorReadingRepositoryMock = new Mock<ISensorReadingRepository>();

            _sensorReadingsController = new SensorReadingsController(_sensorReadingRepositoryMock.Object);
        }

        [Fact]
        public void GetAllShouldReturnEmptyListWhenNoSensorReadingData()
        {
            var result = _sensorReadingsController.GetAll();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetAllShouldReturnAllSensorReadingData()
        {
            _sensorReadingRepositoryMock.Setup(scr => scr.Get(null, null, It.IsAny<float>()))
                .Returns(SensorReadingsTestData);

            var result = _sensorReadingsController.GetAll();

            Assert.NotNull(result);
            Assert.Equal(SensorReadingsTestData.Count(), result.Count());
        }

        public static IEnumerable<SensorReadingDto> SensorReadingsTestData => new List<SensorReadingDto>() {
                                    new SensorReadingDto
                                    {
                                        Hardware = new HardwareDto(){ Name="Hardware"},
                                        Sensor = new SensorDto(){ Name="Sensor"},
                                        TimeStamp = new DateTime(2020, 6, 9, 14, 54, 32),
                                        Value = 1
                                    },
                                    new SensorReadingDto
                                    {
                                        Hardware = new HardwareDto(){ Name="Hardware"},
                                        Sensor = new SensorDto(){ Name="Sensor"},
                                        TimeStamp = new DateTime(2020, 6, 9, 14, 58, 10),
                                        Value = 1
                                    },
                                    new SensorReadingDto
                                    {
                                        Hardware = new HardwareDto(){ Name="Hardware2"},
                                        Sensor = new SensorDto(){ Name="Sensor"},
                                        TimeStamp = new DateTime(2020, 6, 9, 14, 54, 32),
                                        Value = 1
                                    },
                                    new SensorReadingDto
                                    {
                                        Hardware = new HardwareDto(){ Name="Hardware2"},
                                        Sensor = new SensorDto(){ Name="Sensor"},
                                        TimeStamp = new DateTime(2020, 6, 9, 14, 58, 15),
                                        Value = 1
                                    },
                                    new SensorReadingDto
                                    {
                                        Hardware = new HardwareDto(){ Name="Hardware3"},
                                        Sensor = new SensorDto(){ Name="Sensor2"},
                                        TimeStamp = new DateTime(2020, 6, 9, 14, 54, 32),
                                        Value = 1
                                    } };
    }
}
