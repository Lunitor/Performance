using Lunitor.Shared.Dto;

namespace Lunitor.Core.Models
{
    public static class MappingExtensions
    {
        public static SensorReadingDto Map(this SensorReading sensorReading)
        {
            return new SensorReadingDto
            {
                Hardware = sensorReading.Hardware.Map(),
                Sensor = sensorReading.Sensor.Map(),
                TimeStamp = sensorReading.TimeStamp,
                Value = sensorReading.Value
            };
        }

        public static HardwareDto Map(this Hardware hardware)
        {
            return new HardwareDto
            {
                Name = hardware.Name,
                Type = hardware.Type
            };
        }

        public static SensorDto Map(this Sensor sensor)
        {
            return new SensorDto
            {
                HardwareName = sensor.Hardware.Name,
                Name = sensor.Name,
                Type = sensor.Type,
                MaxValue = sensor.MaxValue,
                MinValue = sensor.MinValue
            };
        }
    }
}
