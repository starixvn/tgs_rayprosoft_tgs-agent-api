using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TGS.Apis.LogApi
{
    public class Program
    {
        //public static void Main(string[] args)
        //{
        //    BaseWorkersStartup.CreateHostBuilder<Startup, LogApiFactory>(args)
        //        .Build()
        //        .Run();
        //}


        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
        ;
    }
}
