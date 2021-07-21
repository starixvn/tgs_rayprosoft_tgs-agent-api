using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Promotion.Models.PromotionBonus
{
    public class CreatePromotionBonusWeeklyRequest : IRequest<MessageResponse>
    {
        public string? From { get; set; }

        public string? To { get; set; }
    }
}