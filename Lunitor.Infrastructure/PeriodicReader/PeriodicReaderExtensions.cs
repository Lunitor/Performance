using Lunitor.Core;
using Lunitor.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lunitor.Infrastructure.PeriodicReader
{
    public static class PeriodicReaderExtensions
    {
        public static void AddPeriodicReaderHostedService(this IServiceCollection services, IConfiguration configuration)
        {
            PeriodicReaderConfiguration.Periodicity =  configuration.GetValue<int>(ConfigurationConstants.PeriodicityKey);

            services.AddSingleton<IHardwareMonitor, HardwareMonitor>();
            services.AddHostedService<PeriodicReader>();
        }
    }
}
