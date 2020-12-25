using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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
    }
}