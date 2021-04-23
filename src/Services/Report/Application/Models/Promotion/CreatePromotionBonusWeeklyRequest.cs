using MediatR;

namespace TGS.Services.Report.Application.Models.Promotion
{
    public class CreatePromotionBonusWeeklyRequest : IRequest<CreatePromotionBonusResponse>
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}