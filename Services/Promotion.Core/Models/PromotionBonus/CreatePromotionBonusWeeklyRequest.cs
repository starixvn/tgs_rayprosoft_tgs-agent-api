using MediatR;

namespace TGS.Services.PromotionCore.Models.PromotionBonus
{
    public class CreatePromotionBonusWeeklyRequest : IRequest<CreatePromotionBonusResponse>
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}