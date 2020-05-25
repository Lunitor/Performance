using System;

namespace Lunitor.HardwareMonitorAPI
{
    public class SensorReading
    {
        public DateTime TimeStamp { get; set; }
        public Hardware Hardware { get; set; }
        public Sensor Sensor { get; set; }
        public float? Value { get; set; }
    }
}
