using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Report.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddReportRepository(this IServiceCollection services,
            IConfiguration configuration, string connectionName)
        {
            services.AddRepository<IReportRepository, ReportRepository>(configuration, connectionName);

            return services;
        }
    }
}