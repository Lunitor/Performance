using System.Collections.Generic;
using System.Text;

namespace Lunitor.DataReader.UnitTests
{
    static class HardwareInfoLogTestData
    {
        public static string[] ParamNames { get; private set; } = new[] { "CPU temperature", "GPU power", "RAM usage" };
        public static double[] ParamMins { get; private set; } = new[] { 0.0, 0.0, 0.0 };
        public static double[] ParamMaxs { get; private set; } = new[] { 100.0, 150.0, 16000.0 };
        public static string[] ParamUoMs { get; private set; } = new[] { "°C", "%", "MB" };
        public static List<double[]> Values { get; private set; } = new List<double[]>
            {
                new[]{ 30.2, 40.0, 560.56 },
                new[]{ 32.4, 42.0, 887.23 },
                new[]{ 34.7, 45.0, 1123.34 }
            };
        public static List<double?[]> ValuesWithNA { get; private set; } = new List<double?[]>
            {
                new double?[]{ 30.2, 40.0, 560.56 },
                new double?[]{ 32.4, null, 887.23 },
                new double?[]{ null, 45.0, 1123.34 }
            };
        public static List<string> TimeStamps { get; private set; } = new List<string>
            {
                "01-05-2020 18:18:03",
                "01-05-2020 18:18:13",
                "01-05-2020 18:18:22",
            };

        public static IEnumerable<string> FullLog
        {
            get
            {
                var log = new List<string>();
                log.Add(CreateParameterLine());
                log.AddRange(CreateParameterInfoLines());
                log.AddRange(CreateDataLines());

                return log;
            }
        }

        public static IEnumerable<string> FullLogWithNAValues
        {
            get
            {
                var log = new List<string>();
                log.Add(CreateParameterLine());
                log.AddRange(CreateParameterInfoLines());
                log.AddRange(CreateDataLinesWithNAValues());

                return log;
            }
        }

        public static IEnumerable<string> CreateDataLines()
        {
            var dataLines = new List<string>();
            for (int i = 0; i < Values.Count; i++)
            {
                var dataLine = new StringBuilder($"80, {TimeStamps[i]}");
                foreach (var value in Values[i])
                    dataLine.Append($", {value}       ");

                dataLines.Add(dataLine.ToString());
            }

            return dataLines;
        }

        public static IEnumerable<string> CreateDataLinesWithNAValues()
        {
            var dataLines = new List<string>();
            for (int i = 0; i < ValuesWithNA.Count; i++)
            {
                var dataLine = new StringBuilder($"80, {TimeStamps[i]}");
                foreach (var dataValue in ValuesWithNA[i])
                {
                    var valueString = (dataValue.HasValue) ? dataValue.Value.ToString() : "N/A";
                    dataLine.Append($", {valueString}       ");
                }

                dataLines.Add(dataLine.ToString());
            }

            return dataLines;
        }

        public static string CreateParameterLine()
        {
            StringBuilder paramLine = new StringBuilder("02, 01/02/2020 18:21:12");
            foreach (var paramName in ParamNames)
                paramLine.Append($", {paramName}");
            return paramLine.ToString();
        }

        public static IEnumerable<string> CreateParameterInfoLines()
        {
            var paraminfoLines = new List<string>();

            for (int i = 0; i < ParamMins.Length; i++)
                paraminfoLines.Add($"03, 01/02/2020 18:21:12, {i}, {ParamUoMs[i]}, {ParamMins[i]}, {ParamMaxs[i]}");

            return paraminfoLines;
        }
    }
}
