// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Outgoing
{
    public record GetBetHistoryResponse : BaseResponse
    {
        [JsonPropertyName("result")]
        public GameResult[] Results { get; init; }
    }
}
