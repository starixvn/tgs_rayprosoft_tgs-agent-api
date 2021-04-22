using MediatR;

namespace TGS.Services.Report.Application.Models.Promotion
{
    public class CreatePromotionBonusMonthlyRequest : IRequest<PromotionBonusPlayerModel[]>
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}