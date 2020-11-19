using Amazon.CloudWatchLogs;
using Amazon.S3;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TGS.Apis.FileService.Extensions
{
    public static class AwsServiceExtensions
    {
        public static IServiceCollection AddAwsService(this IServiceCollection services,
            IConfiguration configuration, IWebHostEnvironment environment, string configSectionName = "aws")
        {
            services.Configure<AwsSettings>(configuration.GetSection(configSectionName));
            var awsSettings = configuration.GetSection(configSectionName).Get<AwsSettings>();
            configuration.Bind(awsSettings);
            services.AddSingleton(awsSettings);

            if (environment.IsDevelopment())
            {
                services.AddSingleton<IAmazonS3>(provider =>
                {
                    var settings = provider.GetService<AwsSettings>();
                    return new AmazonS3Client(new AmazonS3Config
                    {
                        UseHttp = true,
                        ServiceURL = settings?.S3.ServiceUrl,
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
            }
            else
            {
                services.AddDefaultAWSOptions(configuration.GetAWSOptions());
                services.AddAWSService<IAmazonS3>();
                services.AddAWSService<IAmazonCloudWatchLogs>();
            }

            return services;
        }
    }
}