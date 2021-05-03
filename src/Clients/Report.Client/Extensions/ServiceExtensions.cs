using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Report.Client.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        ///     Dependency Injection of Operations, need to add the following dependencies beforehand:
        ///     1. AddOperations() in Infrastructures.Operations
        ///     2. AddApiClient() in Infrastructure.ApiClient
        /// </summary>
        public static IServiceCollection AddReportClientServices(this IServiceCollection services,
            IConfiguration configuration, string configSectionName = "Report")
        {
            services.Configure<ReportSettings>(configuration.GetSection(configSectionName));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<ReportSettings>>().Value);
            services.AddHttpClient<IReportAggregateClient, ReportAggregateClient>((sp, c) =>
            {
                var settings = sp.GetRequiredService<ReportSettings>();
                c.BaseAddress = new(settings.Url);
            });
            services.AddHttpClient<IReportClient, ReportClient>((sp, c) =>
            {
                var settings = sp.GetRequiredService<ReportSettings>();

                c.Timeout = TimeSpan.FromHours(1);
                c.BaseAddress = new(settings.Url);
            });

            return services;
        }
    }
}