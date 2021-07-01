using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Commons
{
    public record EvolutionGameUrlPlayer
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("update")]
        public bool Update { get; set; }
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }
        [JsonPropertyName("country")]
        public string Country { get; set; }
        [JsonPropertyName("language")]
        public string Language { get; set; }
        [JsonPropertyName("currency")]
        public string Currency { get; set; }
        [JsonPropertyName("session")]
        public EvolutionGameUrlPlayerSession Session { get; set; }
        [JsonPropertyName("group")]
        public EvolutionGameUrlPlayerGroup Group { get; set; }
    }
}
