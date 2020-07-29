using Lunitor.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Lunitor.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void AddHarwareMonitorAPI(this IServiceCollection services)
        {
            services.AddSingleton<IHardwareMonitor, HardwareMonitor>();
        }
    }
}
