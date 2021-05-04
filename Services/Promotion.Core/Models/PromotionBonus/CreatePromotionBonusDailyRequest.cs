using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Promotion.Core.Models.PromotionBonus
{
    public class CreatePromotionBonusDailyRequest : IRequest<MessageResponse>
    {
        public string Date { get; set; }
    }
}