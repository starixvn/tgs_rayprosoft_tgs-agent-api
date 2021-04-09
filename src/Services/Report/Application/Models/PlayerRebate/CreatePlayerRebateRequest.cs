using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.PlayerRebate
{
    public class CreatePlayerRebateRequest : IRequest<MessageResponse>
    {
        public string Date { get; set; }
    }
}