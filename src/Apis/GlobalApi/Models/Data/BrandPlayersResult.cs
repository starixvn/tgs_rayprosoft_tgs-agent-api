using TGS.Core.Models.Enums;

namespace TGS.Apis.GlobalApi.Models.Data
{
    public record BrandPlayersResult
    {
        public string PlayerCode { get; init; }
        public string Username { get; init; }
        public PlayerStatus Status { get; init; }
    }
}