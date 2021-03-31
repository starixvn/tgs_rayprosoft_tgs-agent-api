using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record PlaceBetResponse : TransactionResponse
    {
        [JsonPropertyName("usedPromo")]
        public decimal UsedPromo { get; init; }
    }
}