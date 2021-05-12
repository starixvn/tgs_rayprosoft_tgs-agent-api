using System;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TGS.Core.Infrastructures.Aws;
using TGS.Core.Infrastructures.Prometheus;
using TGS.Core.Models;

namespace TGS.Apis.FileService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            PrometheusService.StartCollecting();

            var host = CreateHostBuilder(args).Build();
            await CreateBucketAsync(host.Services);
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }

        private static async Task CreateBucketAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;
            var amazonS3 = sp.GetRequiredService<IAmazonS3>();
            var settings = sp.GetRequiredService<AwsSettings>();
            var configuration = sp.GetRequiredService<IConfiguration>();

            if (configuration.GetValue<string>(Consts.EnvironmentVariable.Environment) != "Production" && settings != null && !await amazonS3.DoesS3BucketExistAsync(settings.S3.Bucket))
            {
                var putBucketRequest = new PutBucketRequest
                {
                    BucketName = settings.S3.Bucket,
                    UseClientRegion = true
                };

                await amazonS3.PutBucketAsync(putBucketRequest);
            }
        }
    }
}