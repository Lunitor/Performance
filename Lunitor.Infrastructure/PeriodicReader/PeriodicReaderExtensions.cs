using Lunitor.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Lunitor.Infrastructure.PeriodicReader
{
    public static class PeriodicReaderExtensions
    {
        public static void AddPeriodicReaderHostedService(this IServiceCollection services)
        {
            services.AddSingleton<IHardwareMonitor, HardwareMonitor>();
            services.AddHostedService<PeriodicReader>();
        }
    }
}
