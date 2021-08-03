using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.Promotion.Models.Promotion;

namespace TGS.Services.Promotion.Handlers
{
    public class
        GetPlayerOperationsQueryHandler : Handler<GetJoinedPromotionsRequest, PagedResponse<JoinedPromotionModel>>
    {
        private readonly IPromotionParticipantRepository _service;

        public GetPlayerOperationsQueryHandler(IPromotionParticipantRepository service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        public override Task<PagedResponse<JoinedPromotionModel>> Handle(GetJoinedPromotionsRequest request,
            CancellationToken cancellationToken) =>
            _service.GetPromotionParticipantQueries(q => q.Where(p =>
                    p.CreatedOn >= request.From && p.CreatedOn <= request.To && p.PlayerCode == request.PlayerCode))
                .Select(x => new JoinedPromotionModel
                {
                    CreatedOn = x.CreatedOn,
                    PromotionName = x.Promotion.Name
                }).CreatePagedResponseAsync(request);
    }
}