using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons
{
    public record EvolutionGameUrlConfigUrls
    {
        [JsonPropertyName("cashier")]
        public string Cashier { get; set; }
        [JsonPropertyName("responsibleGaming")]
        public string ResponsibleGaming { get; set; }
        [JsonPropertyName("lobby")]
        public string Lobby { get; set; }
        [JsonPropertyName("sessionTimeout")]
        public string SessionTimeout { get; set; }
    }
}
