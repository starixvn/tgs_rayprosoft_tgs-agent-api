using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Promotion.Models.PromotionDispatch
{
    public class GetPromotionDispatchesRequest : PageableRequest, IRequest<PagedResponse<PromotionDispatchModel>>
    {
        public Guid BrandId { get; set; }

        public DateTimeOffset? From { get; set; }

        public string PromotionGroup { get; set; } = default!;

        public string PromotionName { get; set; } = default!;

        public DateTimeOffset? To { get; set; }

        public string Username { get; set; } = default!;
    }
}
