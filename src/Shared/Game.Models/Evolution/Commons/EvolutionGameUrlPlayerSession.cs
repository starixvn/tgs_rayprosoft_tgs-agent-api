using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons
{
    public record EvolutionGameUrlPlayerSession
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("ip")]
        public string Ip { get; set; }
    }
}
