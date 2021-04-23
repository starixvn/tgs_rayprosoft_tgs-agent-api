using MediatR;

namespace TGS.Services.Report.Application.Models.PlayerRebate
{
    public class CreatePlayerRebateRequest : IRequest<CreatePlayerRebateResponse>
    {
        public string Date { get; set; }
    }
}