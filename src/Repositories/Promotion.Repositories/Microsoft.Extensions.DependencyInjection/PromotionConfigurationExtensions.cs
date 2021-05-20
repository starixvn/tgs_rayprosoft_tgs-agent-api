using TGS.Services.Promotion.Configuration;
using TGS.Promotion.Repositories;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PromotionConfigurationExtensions
    {
        public static PromotionConfiguration AddTgsPromotionRepositories(this PromotionConfiguration configuration)
        {
            return configuration
                .AddPromotionRepository<PromotionRepository>()
                .AddPromotionBonusRepository<PromotionBonusRepository>()
                .AddPromotionParticipantRepository<PromotionParticipantRepository>()
                .AddSettingRepository<SettingRepository>()
                .AddPlayerDataRepository<RedisPlayerDataRepository>();
        }
    }
}
