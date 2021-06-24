using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Promotion.Models.Promotion;
using TGS.Services.Promotion.Properties;

namespace TGS.Services.Promotion.Handlers
{
    public class GetPromotionsQueryHandler : Handler<GetPromotionsRequest,
        PagedResponse<PromotionModel>>
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IPlayerService _playerService;
        private readonly IPromotionRepository _service;

        public GetPromotionsQueryHandler(IPromotionRepository service, IPlayerService playerService,
            IJsonSerializationProvider jsonSerializationProvider,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
            _playerService = playerService;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        public override async Task<PagedResponse<PromotionModel>> Handle(
            GetPromotionsRequest request,
            CancellationToken cancellationToken)
        {
            var player = await _playerService.GetPlayerAsync(x => x.PlayerCode == request.PlayerCode)
                .ConfigureAwait(false);

            if (player is null && request.PlayerCode.IsNotNullOrEmpty())
            {
                return CreateInvalidResponse<PagedResponse<PromotionModel>>(Player.NotFound);
            }

            var promotions = await _service.GetPromotionQueries()
                .Where(x => x.BrandId == request.BrandId)
                .Where(request.Status, x => x.Status == request.Status)
                .Where(request.Name, x => x.Name == request.Name)
                .Where(request.PlayerCode,
                    x => x.PromotionVipLevels.Count == 0 ||
                        x.PromotionVipLevels.Select(p => p.Level).Contains(player.Vip.Level))
                .Where(
                    request.CurrentDate,
                    x => x.Status == SwitchStatus.Enabled
                        && x.From <= request.CurrentDate
                        && x.To > request.CurrentDate)
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
                    LocalizationString = x.Localization
                }).CreatePagedResponseAsync(request).ConfigureAwait(true);

            foreach (var promotionsItem in promotions.Items)
            {
                if (promotionsItem.LocalizationString.IsNotNullOrEmpty())
                {
                    if (request.Language.IsNotNullOrEmpty())
                    {
                        var localization = _jsonSerializationProvider.DeserializeFromString<Dictionary<string, object>>(
                            promotionsItem
                                .LocalizationString);
                        if (localization.TryGetValue(request.Language, out var languageObject))
                        {
                            promotionsItem.LocalizationString = languageObject.ToString();
                            promotionsItem.Localization = JsonDocument.Parse(promotionsItem.LocalizationString!);
                        }

                        continue;
                    }

                    promotionsItem.Localization = JsonDocument.Parse(promotionsItem.LocalizationString!);
                }
            }

            return promotions;
        }
    }
}