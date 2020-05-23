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
        private readonly int _periodicity;

        public PeriodicReader(ILogger<PeriodicReader> logger, IConfiguration configuration)
        {
            _logger = logger;
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

                }
                catch (Exception ex)
                {
                }

                await Task.Delay(_periodicity*1000, stoppingToken);
            }
        }
    }
}
