using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Enums;
using TGS.Services.Promotion.Models.Promotion;
using TGS.ServiceUtility;

namespace TGS.Services.Promotion.Services
{
    internal class GetValidPromotionsQueryService : IQueryService<GetValidPromotionsQuery, PromotionModel>
    {
        private readonly IPromotionRepository _promotionRepository;

        public GetValidPromotionsQueryService(
            IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository ?? throw new ArgumentNullException(nameof(promotionRepository));
        }

        public IAsyncEnumerable<PromotionModel> QueryAsync(GetValidPromotionsQuery query, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.Now;

            var promotionQueryable = _promotionRepository.GetPromotionQueries()
                .Where(x => x.BrandId == query.BrandId)
                .Where(x => x.Unlimited || x.TotalAmountLimit > x.AmountUsed)
                .Where(x => x.Status == SwitchStatus.Enabled
                    && x.From <= now
                    && x.To > now)
                .Select(x => new PromotionModel
                {
                    Id = x.Id,
                    BrandId = x.BrandId,
                    From = x.From,
                    To = x.To,
                    Status = x.Status,
                    Name = x.Name,
                    AutoSentPlayerBonus = x.AutoSentPlayerBonus,
                    PromotionType = x.PromotionType,
                    BonusAmountLimit = x.BonusAmountLimit,
                    AmountUsed = x.AmountUsed,
                    Unlimited = x.Unlimited,
                    GameType = x.GameType,
                    Percentage = x.Percentage,
                    TotalAmountLimit = x.TotalAmountLimit,
                    TurnoverMultiple = x.TurnoverMultiple,
                    GameProviderIds = x.PromotionGameProviders.Select(g => g.GameProviderId).ToArray(),
                    VipLevels = x.PromotionVipLevels.Select(v => v.Level).OrderBy(level => level).ToArray(),
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn,
                    CreatedByUsername = x.CreatedByUsername,
                    UpdatedByUsername = x.UpdatedByUsername,
                    SubSubject = x.SubSubject,
                    Content = x.Content,
                    Terms = x.Terms,
                    DesktopBanner = x.DesktopBanner,
                    MobileBanner = x.MobileBanner,
                    Localization = x.Localization == null ? null : JsonDocument.Parse(x.Localization, new())
                });

            return promotionQueryable.ToAsyncEnumerable();
        }
    }
}
