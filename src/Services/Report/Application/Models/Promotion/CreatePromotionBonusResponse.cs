using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Promotion
{
    public class CreatePromotionBonusResponse : MessageResponse
    {
        public PromotionBonusModel[] PromotionBonus { get; set; }
    }
}