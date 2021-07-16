using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Promotion.Models.Promotion
{
    public class GetJoinedPromotionsRequest : PageableRequest, IRequest<PagedResponse<JoinedPromotionModel>>
    {
        public string PlayerCode { get; init; }
        public DateTimeOffset From { get; init; }
        public DateTimeOffset To { get; init; }
    }
}