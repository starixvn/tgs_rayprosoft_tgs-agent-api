using Microsoft.Extensions.DependencyInjection;

namespace Bonus.Repositories
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBonusRepositoriesServices(this IServiceCollection services)
        {
            services.AddTransient<ISentBonus, SentBonus>();

            return services;
        }
    }
}