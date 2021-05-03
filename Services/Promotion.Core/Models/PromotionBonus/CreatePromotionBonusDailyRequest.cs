using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.PromotionCore.Models.PromotionBonus
{
    public class CreatePromotionBonusDailyRequest : IRequest<MessageResponse>
    {
        public string Date { get; set; }
    }
}