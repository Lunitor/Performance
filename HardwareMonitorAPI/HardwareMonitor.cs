using LibreHardwareMonitor.Hardware;
using System;

namespace HardwareMonitorAPI
{
    public class HardwareMonitor
    {
        private readonly Computer _computer;
        private readonly IVisitor _visitor;

        internal class CustomVisitor : IVisitor
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
            _visitor = new CustomVisitor();
            _computer = new Computer();
        }

        public void Start()
        {
            _computer.Open();
            _computer.IsCpuEnabled = true;
            _computer.Accept(_visitor);
        }

        public void Stop()
        {
            _computer.Close();
        }

        public void PrintStats()
        {
            for (int i = 0; i < _computer.Hardware.Length; i++)
            {
                if (_computer.Hardware[i].HardwareType == HardwareType.Cpu)
                {
                    for (int j = 0; j < _computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (_computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                            Console.WriteLine($"{_computer.Hardware[i].Sensors[j].Name}: {_computer.Hardware[i].Sensors[j].Value?.ToString()}");
                    }
                }
            }
        }
    }
}
