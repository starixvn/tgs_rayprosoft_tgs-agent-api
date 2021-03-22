using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.PragmaticPlay.Common;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record BalanceResponse : ErrorResponse
    {
        [JsonPropertyName("cash")]
        public decimal Balance { get; init; }

        [JsonPropertyName("bonus")]
        public decimal Bonus { get; init; }
    }
}