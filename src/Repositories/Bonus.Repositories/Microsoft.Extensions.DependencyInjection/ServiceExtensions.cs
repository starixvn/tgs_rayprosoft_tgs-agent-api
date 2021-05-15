using Bonus.Repositories;
using TGS.Services.Bonus;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBonusRepositoriesServices(this IServiceCollection services)
        {
            services.AddTransient<IPlayerBonus, PlayerBonusRepository>();
            services.AddTransient<IAgentDividend, AgentDividendRepository>();

            return services;
        }
    }
}