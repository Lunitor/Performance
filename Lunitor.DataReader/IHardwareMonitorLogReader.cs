using Lunitor.DataReader.Models;
using System.Collections.Generic;

namespace Lunitor.DataReader
{
    internal interface IHardwareMonitorLogReader
    {
        Dictionary<Parameter, List<Data>> Read(IEnumerable<string> log);
    }
}