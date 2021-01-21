using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.Hangfire;
using TGS.Services.BackOffice.Application.Extensions;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.Hangfire.Jobs;

namespace TGS.Worker.Schedulers
{
    public class Program
    {
        private static IConfigurationRoot Configuration;

        public static async Task Main(string[] args)
        {
            Configuration = InitConfiguration.Init();
            var config = Configuration.Get<AppConfig>();

            var hostBuilder = new HostBuilder()
                .ConfigureServices((_, services) =>
                {
                    ConfigureServices(services);
                });

            var host = hostBuilder.UseConsoleLifetime().Build();
            using var serviceScope = host.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            GlobalConfiguration.Configuration
                .UseActivator(
                    new BrandGameProviderScheduleActivator(serviceProvider.GetService<ITgsRepository>(),
                        serviceProvider.GetService<IBrandGameProviderService>()))
                .UseColouredConsoleLogProvider();

            RecurringJob.Jobs(serviceProvider);

            await BackgroundJob.StartAsync(config, host);
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTgsRepository(Configuration, "Tgs")
                .AddBackOfficeApplicationServices()
                .AddScheduler(Configuration);
        }
    }
}