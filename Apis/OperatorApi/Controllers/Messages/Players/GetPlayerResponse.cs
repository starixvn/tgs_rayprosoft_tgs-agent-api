using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Controllers.Messages.Players
{
    public class GetPlayerResponse : MessageResponse
    {
        public PlayerModel Player { get; set; }
    }
}
