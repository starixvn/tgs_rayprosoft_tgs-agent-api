using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TGS.Core.Infrastructures.Aws;

namespace TGS.Core.Infrastructures.HealthChecks
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddRedis(this IHealthChecksBuilder builder, IConfiguration configuration,
            string configurationName = "RedisConfigurationString", string name = null,
            HealthStatus? failureStatus = null, IEnumerable<string> tags = null,
            TimeSpan? timeout = null)
        {
            var connectionString = configuration.GetConnectionString(configurationName);
            builder.AddRedis(connectionString, name, failureStatus, tags, timeout);
            return builder;
        }

        public static IHealthChecksBuilder AddPostgres(this IHealthChecksBuilder builder, IConfiguration configuration,
            string configurationName, string name = null, HealthStatus? failureStatus = null,
            IEnumerable<string> tags = null, TimeSpan? timeout = null)
        {
            var connectionString = configuration.GetConnectionString(configurationName);
            builder.AddNpgSql(connectionString, name: name, failureStatus: failureStatus, tags: tags, timeout: timeout);
            return builder;
        }

        public static IHealthChecksBuilder AddNats(this IHealthChecksBuilder builder, string name = "nats",
            HealthStatus? failureStatus = null, IEnumerable<string> tags = null, TimeSpan? timeout = null)
        {
            builder.AddCheck<NatsHealthCheck>(name, failureStatus, tags, timeout);
            return builder;
        }

        public static IHealthChecksBuilder AddMongoDb(this IHealthChecksBuilder builder, IConfiguration configuration,
            string sectionName, string name = "mongodb", HealthStatus? failureStatus = null,
            IEnumerable<string> tags = null, TimeSpan? timeout = null)
        {
            var section = configuration.GetSection(sectionName);
            var connectionString = section.GetValue<string>("ConnectionString");
            builder.AddMongoDb(connectionString, name, failureStatus, tags, timeout);
            return builder;
        }

        public static IHealthChecksBuilder AddAwsS3(this IHealthChecksBuilder builder, string name = null,
            HealthStatus? failureStatus = null, IEnumerable<string> tags = null, TimeSpan? timeout = null)
        {
            name ??= "AwsS3";
            builder.AddS3(sp =>
            {
                var setting = sp.GetRequiredService<AwsSettings>();
                return new()
                {
                    BucketName = setting.Project.Bucket,
                    AccessKey = setting.AccessKeyId,
                    SecretKey = setting.SecretAccessKey,
                    S3Config = new()
                    {
                        UseHttp = setting.UseHttp,
                        ServiceURL = setting.Project.ServiceUrl,
                        ForcePathStyle = true
                    }
                };
            }, $"{name}_public", failureStatus, tags, timeout);

            builder.AddS3(sp =>
            {
                var setting = sp.GetRequiredService<AwsSettings>();
                return new()
                {
                    BucketName = setting.Secret.Bucket,
                    AccessKey = setting.AccessKeyId,
                    SecretKey = setting.SecretAccessKey,
                    S3Config = new()
                    {
                        UseHttp = setting.UseHttp, ServiceURL = setting.Secret.ServiceUrl, ForcePathStyle = true
                    }
                };
            }, $"{name}_private", failureStatus, tags, timeout);

            return builder;
        }
    }
}