using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Promotion.Core.Models.PromotionBonus
{
    public class CreatePromotionBonusMonthlyRequest : IRequest<MessageResponse>
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}