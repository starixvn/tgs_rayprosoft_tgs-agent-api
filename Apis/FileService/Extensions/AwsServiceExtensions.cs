using System;
using Amazon.CloudWatchLogs;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TGS.Apis.FileService.Extensions
{
    public static class AwsServiceExtensions
    {
        public static IServiceCollection AddAwsService(this IServiceCollection services,
            IConfiguration configuration, string configSectionName = "aws")
        {
            services.Configure<AwsSettings>(configuration.GetSection(configSectionName));
            var awsSettings = configuration.GetSection(configSectionName).Get<AwsSettings>();
            configuration.Bind(awsSettings);
            services.AddSingleton(awsSettings);

            services.AddSingleton<IAmazonS3>(provider =>
            {
                var settings = provider.GetService<AwsSettings>();
                if (settings == null)
                {
                    throw new Exception("can't get aws settings");
                }

                return new AmazonS3Client(settings.AccessKeyId, settings.SecretAccessKey, new AmazonS3Config
                {
                    UseHttp = settings.UseHttp,
                    ServiceURL = settings.S3.ServiceUrl,
                    ForcePathStyle = true
                });
            });
            services.AddSingleton<IAmazonCloudWatchLogs>(provider =>
            {
                var settings = provider.GetService<AwsSettings>();
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