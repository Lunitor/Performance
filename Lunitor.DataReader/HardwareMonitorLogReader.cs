using Ardalis.GuardClauses;
using Lunitor.DataReader.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lunitor.DataReader
{
    internal class HardwareMonitorLogReader
    {
        private readonly ILogger<HardwareMonitorLogReader> _logger;

        private int lineCount;

        public HardwareMonitorLogReader(ILogger<HardwareMonitorLogReader> logger)
        {
            _logger = logger;
        }

        public Dictionary<Parameter, List<Data>> Read(string[] log)
        {
            Guard.Against.Null(log, nameof(log));

            Dictionary<Parameter, List<Data>> parameters = new Dictionary<Parameter, List<Data>>();

            lineCount = 0;
            foreach (var line in log)
            {
                lineCount++;
                var parts = line.Split(",").ToList();
                var lineType = parts[0];

                switch (lineType)
                {
                    case "00": break;
                    case "01": break;
                    case "02": AddNewParameters(parameters, parts); break;
                    case "03": FillParametersAttributes(parameters, parts); break;
                    case "80": AddDataUnderParamters(parameters, parts);
                        break;
                    default: throw new InvalidOperationException($"Unknown line type {lineType} at line: {lineCount}");
                }
            }

            return parameters;
            
        }

        private void AddDataUnderParamters(Dictionary<Parameter, List<Data>> parameters, List<string> parts)
        {
            for (int i = 2; i < parts.Count - 1; i++)
            {
                var data = parameters.Values.ElementAt(i - 2);
                try
                {
                    var timeStamp = DateTime.ParseExact(parts[1], "dd-mm-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    var value = double.Parse(parts[i]);

                    data.Add(new Data(timeStamp, value));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Parsing error at line {lineCount}", ex);
                }
            }
        }

        private void FillParametersAttributes(Dictionary<Parameter, List<Data>> parameters, List<string> parts)
        {
            var parameterIndex = int.Parse(parts[2]);
            var parameter = parameters.Keys.ElementAt(parameterIndex);

            parameter.UnitofMeasure = parts[3];
            parameter.Min = double.Parse(parts[4]);
            parameter.Max = double.Parse(parts[5]);
        }

        private void AddNewParameters(Dictionary<Parameter, List<Data>> parameters, List<string> parts)
        {
            for (int i = 2; i < parts.Count - 1; i++)
            {
                if (parameters.Keys.FirstOrDefault(p => p.Name == parts[i]) == null)
                    parameters.TryAdd(new Parameter(parts[i]), new List<Data>());
            }
        }
    }
}
