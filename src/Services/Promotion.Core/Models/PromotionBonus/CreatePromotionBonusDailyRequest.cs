using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Promotion.Models.PromotionBonus
{
    public class CreatePromotionBonusDailyRequest : IRequest<MessageResponse>
    {
        public string Date { get; set; }
    }
}
