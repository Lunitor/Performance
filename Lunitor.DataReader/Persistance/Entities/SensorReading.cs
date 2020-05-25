using System;

namespace Lunitor.DataReader.Persistance.Entities
{
    class SensorReading
    {
        public DateTime TimeStamp { get; set; }
        public Hardware Hardware { get; set; }
        public Sensor Sensor { get; set; }
        public float Value { get; set; }
    }
}
