using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons
{
    public record EvolutionStandardResponse : EvolutionBaseResponse
    {
        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }
        [JsonPropertyName("bonus")]
        public decimal Bonus { get; set; }
        //[JsonPropertyName("retransmission")]
        //public bool Retransmission { get; set; } = false;
    }
}
