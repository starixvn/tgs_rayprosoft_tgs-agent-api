using MediatR;

namespace TGS.Services.Report.Application.Models.Promotion
{
    public class CreatePromotionBonusDailyRequest : IRequest<PromotionBonusPlayerModel[]>
    {
        public string Date { get; set; }
    }
}