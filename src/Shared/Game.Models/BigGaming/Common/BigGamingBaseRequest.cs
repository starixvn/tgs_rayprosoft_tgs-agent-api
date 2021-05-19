using System.Text.Json;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BigGaming.Common
{
    public record BigGamingBaseRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("params")]
        public JsonDocument Params { get; set; }
        [JsonPropertyName("jsonrpc")]
        public string Jsonrpc { get; set; }
    }
}
