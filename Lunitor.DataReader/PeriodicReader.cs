using Lunitor.DataReader.Cache;
using Lunitor.HardwareMonitorAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lunitor.DataReader
{
    internal class PeriodicReader : BackgroundService
    {
        private readonly ILogger<PeriodicReader> _logger;
        private readonly int _periodicity;
        private readonly IHardwareMonitor _hardwareMonitor;
        private readonly ISensorReadingCache _sensorReadingCache;

        public PeriodicReader(ILogger<PeriodicReader> logger, IConfiguration configuration, IHardwareMonitor hardwareMonitor, ISensorReadingCache sensorReadingCache)
        {
            _logger = logger;
            _periodicity = configuration.GetValue<int>("Reader:Periodicity");

            _hardwareMonitor = hardwareMonitor;
            _sensorReadingCache = sensorReadingCache;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting with {periodicity}s periodicity", _periodicity);

            _hardwareMonitor.Start(cpu: true, gpu: true, memory: true, storage: true, network: true, motherboard: true, controller: true);

            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _hardwareMonitor.Stop();

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Running at: {time}", DateTimeOffset.Now);
                try
                {
                    var readings = _hardwareMonitor.Read();
                    foreach (var reading in readings)
                    {
                        Console.WriteLine($"{reading.TimeStamp} {reading.Hardware.Type} {reading.Hardware.Name} {reading.Sensor.Type} {reading.Sensor.Name} {reading.Value}");
                    }

                    _sensorReadingCache.Add(readings);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{nameof(HardwareMonitor)} failed to read get hardware data");
                    _hardwareMonitor.Stop();
                }

                await Task.Delay(_periodicity*1000, stoppingToken);
            }
        }
    }
}
