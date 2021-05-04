using Report.Repositories;
using TGS.Services.Bonus;

namespace Microsoft.Extensions.DependencyInjection
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