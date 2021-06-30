using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.Promotion.Models.Promotion;

namespace TGS.Services.Promotion.Handlers
{
    public class GetPromotionsQueryHandler : Handler<GetPromotionsRequest,
        PagedResponse<PromotionModel>>
    {
        private readonly IPromotionRepository _service;

        public GetPromotionsQueryHandler(IPromotionRepository service,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        public override async Task<PagedResponse<PromotionModel>> Handle(
            GetPromotionsRequest request,
            CancellationToken cancellationToken) =>
            await _service.GetPromotionQueries()
                .Where(x => x.BrandId == request.BrandId)
                .Where(request.Status, x => x.Status == request.Status)
                .Where(request.Name, x => x.Name == request.Name)
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
                    Localization = x.Localization == null ? null : JsonDocument.Parse(x.Localization, new())
                }).AsSingleQuery().CreatePagedResponseAsync(request).ConfigureAwait(false);
    }
}