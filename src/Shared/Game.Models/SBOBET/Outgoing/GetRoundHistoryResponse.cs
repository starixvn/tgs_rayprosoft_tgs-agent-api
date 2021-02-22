// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Outgoing
{
    public record GetRoundHistoryResponse : BaseResponse
    {
        [JsonPropertyName("Result")]
        public GameResult[] Results { get; init; }
    }
}
