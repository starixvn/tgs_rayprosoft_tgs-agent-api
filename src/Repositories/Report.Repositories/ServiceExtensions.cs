using Bonus;
using Microsoft.Extensions.DependencyInjection;

namespace Report.Repositories
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddReportRepositoriesServices(this IServiceCollection services)
        {
            services.AddTransient<IReport, UpdateSentOn>();

            return services;
        }
    }
}