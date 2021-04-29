using TGS.Core.Models.Messages;

namespace TGS.Services.PromotionCore.Models.PromotionBonus
{
    public class CreatePromotionBonusResponse : MessageResponse
    {
        public PromotionBonusModel[] PromotionBonus { get; set; }
    }
}