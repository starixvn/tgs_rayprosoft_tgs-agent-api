using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Players
{
    public class GetPlayerResponse : MessageResponse
    {
        public PlayerModel Player { get; set; }
    }
}
