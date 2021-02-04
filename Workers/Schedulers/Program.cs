using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TGS.Core.Infrastructures.Prometheus;

namespace TGS.Workers.Schedulers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PrometheusService.StartCollecting();

            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
