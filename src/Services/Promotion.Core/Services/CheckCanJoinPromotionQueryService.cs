using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Promotion.Services
{
    internal class CheckCanJoinPromotionQueryService : IGetService<CheckCanJoinPromotionQuery, bool>
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly PromotionCheckerFactory _promotionCheckerFactory;

        public CheckCanJoinPromotionQueryService(
            IPromotionRepository promotionRepository,
            PromotionCheckerFactory promotionCheckerFactory)
        {
            _promotionRepository = promotionRepository ?? throw new ArgumentNullException(nameof(promotionRepository));
            _promotionCheckerFactory = promotionCheckerFactory ?? throw new ArgumentNullException(nameof(promotionCheckerFactory));
        }

        public async ValueTask<bool> GetAsync(CheckCanJoinPromotionQuery query, CancellationToken cancellationToken = default)
        {
            var promotionData = await _promotionRepository.GetPromotionByIdAsync(query.PromotionId);

            if (promotionData.BrandId != query.BrandId)
                return false;

            var checker = _promotionCheckerFactory.GetPromotionChecker(promotionData.PromotionType);

            return await checker.CheckAsync(query.PlayerCode).ConfigureAwait(false);
        }
    }
}
