using TGS.Core.Models.Messages;

namespace TGS.Services.Promotion.Core.Models.PromotionBonus
{
    public class CreatePromotionBonusResponse : MessageResponse
    {
        public PromotionBonusModel[] PromotionBonus { get; set; }
    }
}