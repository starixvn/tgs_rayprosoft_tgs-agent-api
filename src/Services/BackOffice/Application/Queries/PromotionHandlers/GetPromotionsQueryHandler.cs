using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.Promotion.Core;
using TGS.Services.Promotion.Core.Models.Promotion;

namespace TGS.Services.BackOffice.Application.Queries.PromotionHandlers
{
    public class GetPromotionsQueryHandler : Handler<GetPromotionsRequest,
        PagedResponse<PromotionModel>>
    {
        private readonly IPlayerService _playerService;
        private readonly IPromotionService _service;

        public GetPromotionsQueryHandler(IPromotionService service, IPlayerService playerService,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
            _playerService = playerService;
        }

        public override async Task<PagedResponse<PromotionModel>> Handle(
            GetPromotionsRequest request,
            CancellationToken cancellationToken)
        {
            var player = await _playerService.GetPlayerAsync(x => x.PlayerCode == request.PlayerCode);

            if (player is null && request.PlayerCode.IsNotNullOrEmpty())
            {
                return CreateInvalidResponse<PagedResponse<PromotionModel>>(Player.NotFound);
            }

            return await _service.GetPromotionQueries()
                .Where(x => x.BrandId == request.BrandId)
                .Where(request.Status, x => x.Status == request.Status)
                .Where(request.Name, x => x.Name == request.Name)
                .Where(request.PlayerCode, x => x.PromotionVipLevels.Select(p => p.Level).Contains(player.Vip.Level))
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
                    VipLevels = x.PromotionVipLevels.Select(v => v.Level).ToArray(),
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn,
                    CreatedByUsername = x.CreatedByUsername,
                    UpdatedByUsername = x.UpdatedByUsername
                }).CreatePagedResponseAsync(request);
        }
    }
}