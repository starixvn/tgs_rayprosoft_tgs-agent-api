using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        public static void Main(string[] args)
        {
            Configuration = InitConfiguration.Init();
            var config = Configuration.Get<AppConfig>();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            GlobalConfiguration.Configuration
                .UseActivator(
                    new BrandGameProviderScheduleActivator(serviceProvider.GetService<ITgsRepository>(),
                        serviceProvider.GetService<IBrandGameProviderService>()))
                .UseColouredConsoleLogProvider();

            RecurringJob.Jobs(serviceProvider);

            BackgroundJob.Start(config);
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTgsRepository(Configuration, "Tgs")
                .AddBackOfficeApplicationServices()
                .AddScheduler(Configuration)
                .AddHangfireServer();
        }
    }
}