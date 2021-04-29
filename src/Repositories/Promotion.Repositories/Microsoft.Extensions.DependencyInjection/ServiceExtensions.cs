using Promotion.Repositories;
using TGS.Services.PromotionCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPromotionRepositoriesServices(this IServiceCollection services)
        {
            services.AddTransient<IPromotionService, PromotionService>();
            services.AddTransient<IPromotionBonusService, PromotionBonusService>();

            return services;
        }
    }
}