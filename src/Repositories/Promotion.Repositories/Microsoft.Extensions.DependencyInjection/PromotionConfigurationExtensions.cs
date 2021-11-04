using TGS.Promotion.Repositories;
using TGS.Services.Promotion.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PromotionConfigurationExtensions
    {
        public static PromotionConfiguration AddTgsPromotionRepositories(this PromotionConfiguration configuration)
            => configuration
                .AddPromotionRepository<PromotionRepository>()
                .AddPromotionBonusRepository<PromotionBonusRepository>()
                .AddPromotionParticipantRepository<PromotionParticipantRepository>()
                .AddSettingRepository<SettingRepository>()
                .AddPlayerDataRepository<RedisPlayerDataRepository>()
                .AddPromotionPlayerRepository<PromotionPlayerRepository>();
    }
}