using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Promotion.Models.Promotion;
using TGS.ServiceUtility;

namespace TGS.Services.Promotion.Services
{
    internal class GetValidPromotionsQueryService : IGetService<GetValidPromotionsQuery, PagedResponse<PromotionModel>>
    {
        private readonly IPlayerService _playerService;
        private readonly PromotionCheckerFactory _promotionCheckerFactory;
        private readonly IPromotionRepository _promotionRepository;

        public GetValidPromotionsQueryService(
            PromotionCheckerFactory promotionCheckerFactory,
            IPromotionRepository promotionRepository,
            IPlayerService playerService)
        {
            _promotionRepository = promotionRepository ?? throw new ArgumentNullException(nameof(promotionRepository));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _promotionCheckerFactory = promotionCheckerFactory ??
                throw new ArgumentNullException(nameof(promotionCheckerFactory));
        }

        public async ValueTask<PagedResponse<PromotionModel>> GetAsync(GetValidPromotionsQuery request,
            CancellationToken cancellationToken = default)
        {
            var player = await _playerService.GetPlayerAsync(x => x.PlayerCode == request.PlayerCode)
                .ConfigureAwait(false);
            var now = DateTimeOffset.Now;

            var promotions = await _promotionRepository.GetPromotionQueries()
                .Where(x => x.BrandId == request.BrandId)
                .Where(request.PlayerCode,
                    x => x.PromotionVipLevels.Count == 0 ||
                        x.PromotionVipLevels.Select(p => p.Level).Contains(player.Vip.Level))
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
                    LocalizationString = x.Localization,
                    Localization = JsonDocument.Parse(x.Localization, new())
                }).ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var validPromotions = await promotions.Where(x =>
            {
                var checker = _promotionCheckerFactory.GetPromotionChecker(x.PromotionType);

                return checker.CheckAsync(request.PlayerCode);
            });

            return await validPromotions.AsQueryable().CreatePagedResponseAsync(request).ConfigureAwait(false);
        }
    }
}