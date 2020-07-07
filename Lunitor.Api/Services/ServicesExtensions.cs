using Microsoft.Extensions.DependencyInjection;

namespace Lunitor.Api.Services
{
    public static class ServicesExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ISensorReadingService, SensorReadingService>();
        }
    }
}
