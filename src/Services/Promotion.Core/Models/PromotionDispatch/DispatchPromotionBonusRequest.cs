using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Promotion.Models.PromotionDispatch
{
    public class DispatchPlayer
    {
        public string PlayerCode { get; set; } = default!;

        public Guid PromotionBonusId { get; set; }
    }

    public class DispatchPromotionBonusRequest : IRequest<MessageResponse>
    {
        public DispatchPlayer[] Players { get; set; } = default!;
    }
}
