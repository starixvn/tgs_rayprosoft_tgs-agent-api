using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons
{
    public record EvolutionPromoTransactionJackpots
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("winAmount")]
        public decimal WinAmount { get; set; }
    }
}
