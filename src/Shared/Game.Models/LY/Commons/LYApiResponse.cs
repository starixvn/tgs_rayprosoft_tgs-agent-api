using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.LY.Commons
{
    public record LYApiResponse<TData>
    {
        [JsonPropertyName("m")]
        public string Message { get; init; }

        [JsonPropertyName("s")]
        public int SubMethod { get; init; }

        [JsonPropertyName("d")]
        public TData Data { get; init; }
    }
}
