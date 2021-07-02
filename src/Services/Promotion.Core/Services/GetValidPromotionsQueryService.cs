using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Core.Models.Settings;
using TGS.Services.Promotion.Models.Promotion;
using TGS.ServiceUtility;

namespace TGS.Services.Promotion.Services
{
    internal class GetValidPromotionsQueryService : IGetService<GetValidPromotionsQuery, PagedResponse<PromotionModel>>
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IPlayerService _playerService;
        private readonly IPromotionCheckerFactory _promotionCheckerFactory;
        private readonly IPromotionRepository _promotionRepository;
        private readonly ISettingsService _settingsService;

        public GetValidPromotionsQueryService(
            IPromotionCheckerFactory promotionCheckerFactory,
            IPromotionRepository promotionRepository,
            IPlayerService playerService,
            ISettingsService settingsService,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _promotionRepository = promotionRepository ?? throw new ArgumentNullException(nameof(promotionRepository));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _promotionCheckerFactory = promotionCheckerFactory ??
                throw new ArgumentNullException(nameof(promotionCheckerFactory));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _jsonSerializationProvider = jsonSerializationProvider ??
                throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        public async ValueTask<PagedResponse<PromotionModel>> GetAsync(GetValidPromotionsQuery request,
            CancellationToken cancellationToken = default)
        {
            var player = await _playerService.GetPlayerAsync(x => x.PlayerCode == request.PlayerCode)
                .ConfigureAwait(false);
            var now = DateTimeOffset.Now;

            var promotionQueryable = _promotionRepository.GetPromotionQueries()
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
                    Localization = x.Localization == null ? null : JsonDocument.Parse(x.Localization, new())
                });

            if (request.PlayerCode.IsNullOrEmpty())
            {
                return promotionQueryable.CreatePagedResponse(request);
            }

            var promotionsJson = await _settingsService.GetSettingAsync(x => x.Key == SettingConsts.PromotionType);
            var promotionTypeGroups =
                _jsonSerializationProvider.DeserializeFromString<PromotionTypeGroup[]>(promotionsJson.Value);

            var promotionTypes =
                promotionTypeGroups.First(x => x.Name == "Deposit").Types.Select(x => x.Name).ToArray();

            var promotions = await promotionQueryable.AsSingleQuery().ToArrayAsync(cancellationToken);

            var validPromotions = await promotions.Where(x =>
            {
                if (promotionTypes.All(p => x.PromotionType != p))
                {
                    return ValueTask.FromResult(true);
                }

                var checker = _promotionCheckerFactory.GetPromotionChecker(x.PromotionType);

                return checker.CheckAsync(request.PlayerCode);
            }).ConfigureAwait(false);

            return validPromotions.AsQueryable().CreatePagedResponse(request);
        }
    }
}