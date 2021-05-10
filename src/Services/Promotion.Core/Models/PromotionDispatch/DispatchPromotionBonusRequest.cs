using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Promotion.Core.Models.PromotionDispatch
{
    public class DispatchPromotionBonusRequest : IRequest<MessageResponse>
    {
        public DispatchPlayer[] Players { get; set; }
    }

    public class DispatchPlayer
    {
        public Guid PromotionBonusId { get; set; }
        public string PlayerCode { get; set; }
    }
}