using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Promotion;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Queries.PromotionHandlers
{
    public class GetPromotionsQueryHandler : Handler<GetPromotionsRequest,
        PagedResponse<PromotionModel>>
    {
        private readonly IPromotionService _service;

        public GetPromotionsQueryHandler(IPromotionService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        public override Task<PagedResponse<PromotionModel>> Handle(
            GetPromotionsRequest request,
            CancellationToken cancellationToken) =>
            _service.GetPromotionQueries()
                .Select(x => new PromotionModel
                {
                    Id = x.Id,
                    From = x.From,
                    To = x.To,
                    Status = x.Status,
                    Name = x.Name,
                    AutoSentPlayerBonus = x.AutoSentPlayerBonus,
                    PromotionType = x.PromotionType,
                    PromotionAmount = x.BonusAmountLimit,
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