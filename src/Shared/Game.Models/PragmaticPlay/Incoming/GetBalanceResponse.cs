using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record GetBalanceResponse : BalanceResponse
    {
        [JsonPropertyName("currency")]
        public string Currency { get; init; }
    }
}