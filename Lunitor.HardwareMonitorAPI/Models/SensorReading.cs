﻿using System;

namespace Lunitor.HardwareMonitorAPI.Models
{
    public class SensorReading
    {
        public DateTime TimeStamp { get; set; }
        public Hardware Hardware { get; set; }
        public Sensor Sensor { get; set; }
        public float? Value { get; set; }
    }
}