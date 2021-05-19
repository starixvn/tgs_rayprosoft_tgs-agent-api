using System.Text.Json;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BigGaming.Common
{
    public record BigGamingBaseResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("result")]
        public JsonDocument Result { get; set; }
        [JsonPropertyName("error")]
        public BigGamingBaseErrorResponse Error { get; set; }
        [JsonPropertyName("jsonrpc")]
        public string Jsonrpc { get; set; }
    }
}
