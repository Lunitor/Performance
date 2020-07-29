using Lunitor.Core;
using Lunitor.Core.Interfaces.Cache;
using Lunitor.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lunitor.Infrastructure.PeriodicReader
{
    internal class PeriodicReader : BackgroundService
    {
        private readonly ILogger<PeriodicReader> _logger;
        private readonly int _periodicity;
        private readonly IHardwareMonitor _hardwareMonitor;
        private readonly ISensorCacheMutator _sensorCacheMutator;

        public PeriodicReader(ILogger<PeriodicReader> logger,
            IConfiguration configuration,
            IHardwareMonitor hardwareMonitor,
            ISensorCacheMutator sensorCacheMutator)
        {
            _logger = logger;
            _periodicity = configuration.GetValue<int>(ConfigurationConstants.PeriodicityKey);

            _hardwareMonitor = hardwareMonitor;
            _sensorCacheMutator = sensorCacheMutator;
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

                CleanSensorReadingCache();

                ReadSensorData();

                await Task.Delay(_periodicity * 1000, stoppingToken);
            }
        }

        private void ReadSensorData()
        {
            _logger.LogInformation("Sensor reading...");
            try
            {
                var readings = _hardwareMonitor.Read();
                foreach (var reading in readings)
                {
                    Console.WriteLine($"{reading.TimeStamp} {reading.Hardware.Type} {reading.Hardware.Name} {reading.Sensor.Type} {reading.Sensor.Name} {reading.Value}");
                }

                _sensorCacheMutator.Add(readings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(HardwareMonitor)} failed to read hardware data");
            }
        }

        private void CleanSensorReadingCache()
        {
            _logger.LogInformation("Cleaning sensor readings cache ...");
            try
            {
                _sensorCacheMutator.Clean();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to clean sensor readings cache");
            }
        }
    }
}
