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

        public static IHealthChecksBuilder AddSqlServer(this IHealthChecksBuilder builder, IConfiguration configuration,
            string configurationName, string healthQuery = null, string name = null, HealthStatus? failureStatus = null,
            IEnumerable<string> tags = null,
            TimeSpan? timeout = null)
        {
            var connectionString = configuration.GetConnectionString(configurationName);
            builder.AddSqlServer(connectionString, healthQuery, name, failureStatus, tags, timeout);
            return builder;
        }

        public static IHealthChecksBuilder AddNats(this IHealthChecksBuilder builder, string name = "nats",
            HealthStatus? failureStatus = null, IEnumerable<string> tags = null, TimeSpan? timeout = null)
        {
            builder.AddCheck<NatsHealthCheck>(name, failureStatus, tags, timeout);
            return builder;
        }

        public static IHealthChecksBuilder AddMongoDb(this IHealthChecksBuilder builder, IConfiguration configuration,
            string sectionName, string name = "mongodb", HealthStatus? failureStatus = null, IEnumerable<string> tags = null, TimeSpan? timeout = null)
        {
            var section = configuration.GetSection(sectionName);
            var connectionString = section.GetValue<string>("ConnectionString");

            builder.AddMongoDb(connectionString, name, failureStatus, tags, timeout);
            return builder;
        }

        public static IHealthChecksBuilder AddAwsS3(this IHealthChecksBuilder builder, string name = null, HealthStatus? failureStatus = null, IEnumerable<string> tags = null, TimeSpan? timeout = null)
        {
            builder.AddS3(sp =>
            {
                var setting = sp.GetRequiredService<AwsSettings>();
                return new()
                {
                    BucketName = setting.S3.Bucket,
                    AccessKey = setting.AccessKeyId,
                    SecretKey = setting.SecretAccessKey,
                    S3Config = new()
                    {
                        UseHttp = setting.UseHttp,
                        ServiceURL = setting.S3.ServiceUrl,
                        ForcePathStyle = true
                    }
                };

            }, name, failureStatus, tags, timeout);

            return builder;
        }
    }
}