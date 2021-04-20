using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Promotion
{
    public class CreatePromotionBonusRequest : IRequest<MessageResponse>
    {
        public string Date { get; set; }
    }
}