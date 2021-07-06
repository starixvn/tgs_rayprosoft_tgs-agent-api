using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using TGS.Services.Promotion.Models.Promotion;
using TGS.ServiceUtility;

namespace TGS.Services.Promotion.Services
{
    internal class GetValidPromotionsByPlayerQueryService : IQueryService<GetValidPromotionsByPlayerQuery, PromotionModel>
    {
        private readonly IPlayerService _playerService;
        private readonly IPromotionCheckerFactory _promotionCheckerFactory;
        private readonly IQueryService<GetValidPromotionsQuery, PromotionModel> _promotionQueryService;

        public GetValidPromotionsByPlayerQueryService(
            IQueryService<GetValidPromotionsQuery, PromotionModel> promotionQueryService,
            IPlayerService playerService,
            IPromotionCheckerFactory promotionCheckerFactory)
        {
            _promotionQueryService = promotionQueryService ?? throw new ArgumentNullException(nameof(promotionQueryService));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _promotionCheckerFactory = promotionCheckerFactory ?? throw new ArgumentNullException(nameof(promotionCheckerFactory));
        }

        public async IAsyncEnumerable<PromotionModel> QueryAsync(
            GetValidPromotionsByPlayerQuery query,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var player = await _playerService
                .GetPlayerAsync(x => x.PlayerCode == query.PlayerCode)
                .ConfigureAwait(false);

            var promotions = GetPromotionsAsync(
                query.BrandId,
                query.PlayerCode,
                player.Vip.Level,
                cancellationToken);

            await foreach (var promotion in promotions)
            {
                yield return promotion;
            }
        }

        private async IAsyncEnumerable<PromotionModel> GetPromotionsAsync(
            Guid brandId,
            string playerCode,
            int vipLevel,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var promotions = _promotionQueryService
                .QueryAsync(
                    new GetValidPromotionsQuery(brandId),
                    cancellationToken)
                .Where(row => row.VipLevels.Length == 0
                    || row.VipLevels.Contains(vipLevel))
                .ToEnumerable()
                .ToAsyncEnumerable();

            await foreach (var promotion in promotions)
            {
                var checker = _promotionCheckerFactory.GetPromotionChecker(promotion.PromotionType);

                if (await checker.CheckAsync(playerCode).ConfigureAwait(false))
                    yield return promotion;
            }
        }
    }
}
