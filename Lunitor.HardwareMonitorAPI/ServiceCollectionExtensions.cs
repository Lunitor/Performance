using Microsoft.Extensions.DependencyInjection;

namespace Lunitor.HardwareMonitorAPI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddHarwareMonitorAPI(this IServiceCollection services)
        {
            services.AddSingleton<IHardwareMonitor, HardwareMonitor>();
        }
    }
}
