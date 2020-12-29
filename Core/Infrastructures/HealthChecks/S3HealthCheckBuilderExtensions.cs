using System;
using System.Collections.Generic;
using HealthChecks.Aws.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TGS.Core.Infrastructures.HealthChecks
{
    internal static class S3HealthCheckBuilderExtensions
    {
        private const string Name = "Aws S3";

        internal static IHealthChecksBuilder AddS3(this IHealthChecksBuilder builder,
            Func<IServiceProvider, S3BucketOptions> setup, string name = null,
            HealthStatus? failureStatus = default, IEnumerable<string> tags = default, TimeSpan? timeout = default)
        {
            IHealthCheck GetS3BucketOptions(IServiceProvider serviceProvider)
            {
                if (setup == null)
                {
                    return new S3HealthCheck(new());
                }

                var options = setup(serviceProvider);
                return new S3HealthCheck(options);
            }

            return builder.Add(new(
                name ?? Name,
                GetS3BucketOptions,
                failureStatus,
                tags,
                timeout));
        }
    }
}