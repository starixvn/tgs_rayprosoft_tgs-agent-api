using Microsoft.Extensions.DependencyInjection;

namespace Bonus.Repositories
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBonusRepositoriesServices(this IServiceCollection services)
        {
            services.AddTransient<IPlayerBonus, PlayerBonus>();
            services.AddTransient<IAgentDividend, AgentDividend>();

            return services;
        }
    }
}