using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BigGaming.Common
{
    public record BigGamingSignParamsRequest
    {
        [JsonPropertyName("random")]
        public string Random { get; set; }
        [JsonPropertyName("sign")]
        public string Sign { get; set; }
        [JsonPropertyName("sn")]
        public string Sn { get; set; }
    }
}
