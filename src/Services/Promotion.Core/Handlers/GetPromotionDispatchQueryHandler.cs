using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Messages;
using TGS.Core.Models.Settings;
using TGS.Services.Promotion.Models.PromotionDispatch;

namespace TGS.Services.Promotion.Handlers
{
    internal class
        GetPromotionDispatchQueryHandler : Handler<GetPromotionDispatchesRequest, PagedResponse<PromotionDispatchModel>>
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IPromotionBonusRepository _promotionBonusService;
        private readonly ISettingRepository _settingRepository;
        private readonly IPlayerService _tgsPlayerService;
        private readonly IPromotionRepository _tgsPromotionService;

        public GetPromotionDispatchQueryHandler(IPromotionBonusRepository promotionBonusService,
            IPromotionRepository tgsPromotionService,
            IPlayerService tgsPlayerService,
            ISettingRepository settingRepository,
            IJsonSerializationProvider jsonSerializationProvider,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _promotionBonusService = promotionBonusService;
            _tgsPromotionService = tgsPromotionService;
            _tgsPlayerService = tgsPlayerService;
            _settingRepository = settingRepository;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        public override async Task<PagedResponse<PromotionDispatchModel>> Handle(
            GetPromotionDispatchesRequest request,
            CancellationToken cancellationToken)
        {
            var promotionsJson = await _settingRepository.GetSettingAsync(x => x.Key == SettingConsts.PromotionType);
            var promotionTypeGroups =
                _jsonSerializationProvider.DeserializeFromString<PromotionTypeGroup[]>(promotionsJson.Value);

            var promotionGroup = promotionTypeGroups.FirstOrDefault(x => x.Name == request.PromotionGroup);
            var promotionTypes = promotionGroup?.Types.Select(x => x.Name).ToArray();

            var promotion = await _tgsPromotionService.GetPromotionAsync(x => x.Name == request.PromotionName);
            var player = await _tgsPlayerService.GetPlayerAsync(x => x.Username == request.Username);

            var promotionDispatchPages = await _promotionBonusService.GetPromotionBonusPlayersQueries()
                .Where(x => x.PromotionBonus.BrandId == request.BrandId)
                .Where(request.From, x => request.From <= x.PromotionBonus.CreatedOn)
                .Where(request.To, x => request.To >= x.PromotionBonus.CreatedOn)
                .Where(promotion is not null, x => x.PromotionBonus.PromotionId == promotion.Id)
                .Where(player is not null, x => x.PlayerCode == player.PlayerCode)
                .Where(promotionTypes, x => promotionTypes.Contains(x.PromotionBonus.PromotionType))
                .Select(x => new PromotionDispatchModel
                {
                    PromotionBonusId = x.PromotionBonus.Id,
                    PromotionId = x.PromotionBonus.PromotionId,
                    PromotionType = x.PromotionBonus.PromotionType,
                    CreatedOn = x.PromotionBonus.CreatedOn,
                    PlayerCode = x.PlayerCode,
                    BonusAmount = x.BonusAmount,
                    SentOn = x.SentOn
                })
                .CreatePagedResponseAsync(request);

            var playerCodes = promotionDispatchPages.Items.Select(x => x.PlayerCode).ToArray();
            var players = await _tgsPlayerService.GetPlayerQueries().Where(x => playerCodes.Contains(x.PlayerCode))
                .ToArrayAsync(cancellationToken);

            var promotionIds = promotionDispatchPages.Items.Select(x => x.PromotionId).ToArray();
            var promotions = await _tgsPromotionService.GetPromotionQueries().Where(x => promotionIds.Contains(x.Id))
                .ToArrayAsync(cancellationToken);

            promotionDispatchPages.Items =
                promotionDispatchPages.Items.Select(x => new PromotionDispatchModel
                {
                    PromotionBonusId = x.PromotionBonusId,
                    PromotionId = x.PromotionId,
                    PlayerCode = x.PlayerCode,
                    BonusAmount = x.BonusAmount,
                    Username = players.First(p => p.PlayerCode == x.PlayerCode).Username,
                    PromotionName = promotions.First(p => p.Id == x.PromotionId).Name,
                    PromotionGroup =
                        promotionTypeGroups.First(p => p.Types.Select(t => t.Name).Contains(x.PromotionType)).Name,
                    PromotionType = x.PromotionType,
                    CreatedOn = x.CreatedOn,
                    SentOn = x.SentOn
                }).ToArray();

            return promotionDispatchPages;
        }
    }
}
