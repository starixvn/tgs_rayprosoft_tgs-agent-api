using System;
using Amazon.CloudWatchLogs;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TGS.Core.Infrastructures.Aws
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAws(this IServiceCollection services,
            IConfiguration configuration, string configSectionName = "AWS")
        {
            services.Configure<AwsSettings>(configuration.GetSection(configSectionName));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<AwsSettings>>().Value);

            services.AddSingleton<IAmazonS3>(sp =>
            {
                var settings = sp.GetRequiredService<AwsSettings>();
                if (settings == null)
                {
                    throw new ArgumentException("can't get aws settings");
                }

                return new AmazonS3Client(settings.AccessKeyId, settings.SecretAccessKey, new AmazonS3Config
                {
                    UseHttp = settings.UseHttp,
                    ServiceURL = settings.S3.ServiceUrl,
                    ForcePathStyle = true
                });
            });

            services.AddSingleton<IAmazonCloudWatchLogs>(sp =>
            {
                var settings = sp.GetRequiredService<AwsSettings>();
                return new AmazonCloudWatchLogsClient(new AmazonCloudWatchLogsConfig
                {
                    UseHttp = true,
                    ServiceURL = settings?.CloudWatch.ServiceUrl
                });
            });

            return services;
        }
    }
}