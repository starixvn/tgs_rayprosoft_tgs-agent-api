using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public class GetPlayerResponse : MessageResponse
    {
        public PlayerModel Player { get; set; }
    }
}
