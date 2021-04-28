using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.PlayerRebate
{
    public class CreatePlayerRebateResponse : MessageResponse
    {
        public PlayerRebateModel[] PlayerRebates { get; set; }
    }
}