using Bonus;
using Microsoft.Extensions.DependencyInjection;

namespace Report.Repositories
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBonusRepositoriesServices(this IServiceCollection services)
        {
            services.AddTransient<IReport, UpdateSentOn>();

            return services;
        }
    }
}