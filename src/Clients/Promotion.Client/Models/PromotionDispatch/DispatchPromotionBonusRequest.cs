using System;

namespace TGS.Clients.Promotion.Client.Models.PromotionDispatch
{
    public class DispatchPromotionBonusRequest
    {
        public DispatchPlayer[] Players { get; set; }
    }

    public class DispatchPlayer
    {
        public Guid PromotionBonusId { get; set; }
        public string PlayerCode { get; set; }
    }
}