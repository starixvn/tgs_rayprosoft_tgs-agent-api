using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.Promotion
{
    public class GetPromotionsRequest : PageableRequest, IRequest<PagedResponse<PromotionModel>>
    {
        public PromotionType? Type { get; set; }
        public SwitchStatus? Status { get; set; }
        public string Name { get; set; }
    }
}