using MediatR;

namespace TGS.Services.PromotionCore.Models.PromotionBonus
{
    public class CreatePromotionBonusDailyRequest : IRequest<CreatePromotionBonusResponse>
    {
        public string Date { get; set; }
    }
}