using System;

namespace Lunitor.Shared.Dto
{
    public class SensorReadingDto
    {
        public DateTime TimeStamp { get; set; }
        public HardwareDto Hardware { get; set; }
        public SensorDto Sensor { get; set; }
        public float? Value { get; set; }
    }
}
