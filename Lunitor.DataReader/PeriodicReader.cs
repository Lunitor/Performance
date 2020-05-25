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
        private readonly HardwareMonitor _hardwareMonitor;


        public PeriodicReader(ILogger<PeriodicReader> logger, IConfiguration configuration)
        {
            _logger = logger;
            _periodicity = configuration.GetValue<int>("Reader:Periodicity");

            _hardwareMonitor = new HardwareMonitor();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting with {periodicity}s periodicity", _periodicity);

            _hardwareMonitor.Start();

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
                    _hardwareMonitor.PrintStats();
                }
                catch (Exception ex)
                {
                    _hardwareMonitor.Stop();
                }

                await Task.Delay(_periodicity*1000, stoppingToken);
            }
        }
    }
}
