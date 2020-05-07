using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lunitor.DataReader
{
    internal class PeriodicReader : BackgroundService
    {
        private readonly ILogger<PeriodicReader> _logger;
        private readonly IHardwareMonitorLogReader _hardwareLogReader;
        private readonly string _filePath;
        private readonly int _periodicity;

        public PeriodicReader(ILogger<PeriodicReader> logger, IHardwareMonitorLogReader hardwareLogReader, IConfiguration configuration)
        {
            _logger = logger;
            _hardwareLogReader = hardwareLogReader;
            _filePath = configuration.GetValue<string>("Reader:FilePath");
            _periodicity = configuration.GetValue<int>("Reader:Periodicity");
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting with {periodicity}s periodicity", _periodicity);
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Running at: {time}", DateTimeOffset.Now);
                try
                {
                    _logger.LogInformation("Reading hardware log from {filePath}", _filePath);
                    var hardwareLog = await File.ReadAllLinesAsync(_filePath);

                    var data = _hardwareLogReader.Read(hardwareLog);

                    _logger.LogInformation("Hardware log file processed: {parameterCount} paramter with {dataCount} data point/paramter", data.Keys.Count, data.Values.FirstOrDefault()?.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to read/process hardware log file", ex);
                }

                await Task.Delay(_periodicity*1000, stoppingToken);
            }
        }
    }
}
