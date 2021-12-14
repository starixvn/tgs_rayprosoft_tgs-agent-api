using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Avia.Commons
{
    public record AviaBaseResponse<TInfo>
    {
        [JsonPropertyName("success")]
        public int Success { get; init; }
        [JsonPropertyName("msg")]
        public string Message { get; init; }
        [JsonPropertyName("info")]
        public TInfo Info { get; init; }
    }
}
