using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.PromotionCore.Models.Promotions
{
    public class GetPromotionsRequest : PageableRequest, IRequest<PagedResponse<PromotionModel>>
    {
        public string Type { get; set; }
        public SwitchStatus? Status { get; set; }
        public string Name { get; set; }
    }
}