﻿using System.Collections.Generic;

namespace Lunitor.HardwareMonitorAPI
{
    public interface IHardwareMonitor
    {
        IEnumerable<SensorReading> Read();
        void Start(bool cpu = false, bool gpu = false, bool memory = false, bool storage = false, bool network = false, bool motherboard = false, bool controller = false);
        void Stop();
    }
}