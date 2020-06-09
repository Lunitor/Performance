using Lunitor.DataReader.Cache;
using Lunitor.HardwareMonitorAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lunitor.DataReader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    
                    services.AddHostedService<PeriodicReader>();
                    services.AddCache(hostContext.Configuration.GetConnectionString("Redis"));
                    services.AddHarwareMonitorAPI();
                });
    }
}
