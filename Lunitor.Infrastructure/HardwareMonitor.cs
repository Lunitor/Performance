using LibreHardwareMonitor.Hardware;
using Lunitor.Core;
using Lunitor.Core.Models;
using System;
using System.Collections.Generic;

namespace Lunitor.Infrastructure
{
    public class HardwareMonitor : IHardwareMonitor
    {
        private readonly Computer _computer;
        private readonly IVisitor _visitor;

        internal class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }

            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (var subHardware in hardware.SubHardware)
                    subHardware.Accept(this);
            }

            public void VisitParameter(IParameter parameter)
            {
                throw new NotImplementedException();
            }

            public void VisitSensor(ISensor sensor)
            {
                throw new NotImplementedException();
            }
        }

        public HardwareMonitor()
        {
            _visitor = new UpdateVisitor();
            _computer = new Computer();
        }

        public void Start(bool cpu = false, bool gpu = false, bool memory = false, bool storage = false, bool network = false, bool motherboard = false, bool controller = false)
        {
            _computer.Open();

            _computer.IsCpuEnabled = cpu;
            _computer.IsGpuEnabled = gpu;
            _computer.IsMemoryEnabled = memory;
            _computer.IsStorageEnabled = storage;
            _computer.IsNetworkEnabled = network;
            _computer.IsMotherboardEnabled = motherboard;
            _computer.IsControllerEnabled = controller;

            _computer.Accept(_visitor);
        }

        public void Stop()
        {
            _computer.Close();
        }

        public IEnumerable<SensorReading> Read()
        {
            var readings = new List<SensorReading>();

            var timeStamp = DateTime.UtcNow;

            foreach (var hardware in _computer.Hardware)
            {
                hardware.Update();

                var hardwareInfo = GetHardwareInfo(hardware);

                foreach (var sensor in hardware.Sensors)
                {
                    var sensorInfo = GetSensorInfo(hardwareInfo, sensor);

                    readings.Add(new SensorReading
                    {
                        TimeStamp = timeStamp,
                        Hardware = hardwareInfo,
                        Sensor = sensorInfo,
                        Value = sensor.Value
                    });
                }
            }

            return readings;
        }

        private static Sensor GetSensorInfo(Core.Models.Hardware hardwareInfo, ISensor sensor)
        {
            return new Sensor
            {
                Type = sensor.SensorType.ToString(),
                Name = sensor.Name,
                Hardware = hardwareInfo,
                MinValue = sensor.Min,
                MaxValue = sensor.Max
            };
        }

        private static Core.Models.Hardware GetHardwareInfo(IHardware hardware)
        {
            return new Core.Models.Hardware
            {
                Type = hardware.HardwareType.ToString(),
                Name = hardware.Name,
            };
        }
    }
}
