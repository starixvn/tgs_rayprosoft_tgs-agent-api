using TGS.Core.Models.Enums;

namespace TGS.Services.BackOffice.Application.Models.Player
{
    public class PlayersModel
    {
        public string PlayerCode { get; init; }
        public string Username { get; init; }
        public int? VipLevel { get; init; }
        public PlayerStatus Status { get; init; }
        public bool HasAgent { get; init; }
    }
}