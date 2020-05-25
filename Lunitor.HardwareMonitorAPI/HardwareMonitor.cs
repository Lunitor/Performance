using LibreHardwareMonitor.Hardware;
using System;

namespace Lunitor.HardwareMonitorAPI
{
    public class HardwareMonitor
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

        public void PrintStats()
        {
            foreach (var hardware in _computer.Hardware)
            {
                foreach (var sensor in hardware.Sensors)
                {
                    Console.WriteLine($"{sensor.Name} {sensor.Value}");
                }
            }
        }
    }
}
