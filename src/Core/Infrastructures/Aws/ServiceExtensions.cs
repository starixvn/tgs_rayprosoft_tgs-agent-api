using System;
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

            services.AddSingleton<IPublicAmazonS3>(sp =>
            {
                var settings = sp.GetRequiredService<AwsSettings>();
                if (settings == null)
                {
                    throw new ArgumentException("can't get aws settings");
                }

                return new PublicAmazonS3Client(settings.AccessKeyId, settings.SecretAccessKey,
                    new()
                    {
                        UseHttp = settings.UseHttp, ServiceURL = settings.Project.ServiceUrl, ForcePathStyle = true
                    });
            });

            services.AddSingleton<IPrivateAmazonS3>(sp =>
            {
                var settings = sp.GetRequiredService<AwsSettings>();
                if (settings == null)
                {
                    throw new ArgumentException("can't get aws settings");
                }

                return new PrivateAmazonS3Client(settings.AccessKeyId, settings.SecretAccessKey,
                    new() {UseHttp = settings.UseHttp, ServiceURL = settings.Secret.ServiceUrl, ForcePathStyle = true});
            });

            return services;
        }
    }
}