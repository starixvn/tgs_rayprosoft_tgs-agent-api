using Promotion.Repositories;
using TGS.Services.Promotion;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPromotionRepositoriesServices(this IServiceCollection services)
        {
            services.AddTransient<IPromotionRepository, PromotionRepository>();
            services.AddTransient<IPromotionBonusRepository, PromotionBonusRepository>();
            services.AddTransient<IPromotionParticipantRepository, PromotionParticipantRepository>();

            return services;
        }
    }
}