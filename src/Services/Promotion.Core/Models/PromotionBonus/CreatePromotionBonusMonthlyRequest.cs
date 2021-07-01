using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Promotion.Models.PromotionBonus
{
    public class CreatePromotionBonusMonthlyRequest : IRequest<MessageResponse>
    {
        public string From { get; set; } = default!;

        public string To { get; set; } = default!;
    }
}
