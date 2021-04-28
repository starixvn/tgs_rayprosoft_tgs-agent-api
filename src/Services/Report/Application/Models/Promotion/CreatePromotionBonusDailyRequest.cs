using MediatR;

namespace TGS.Services.Report.Application.Models.Promotion
{
    public class CreatePromotionBonusDailyRequest : IRequest<CreatePromotionBonusResponse>
    {
        public string Date { get; set; }
    }
}