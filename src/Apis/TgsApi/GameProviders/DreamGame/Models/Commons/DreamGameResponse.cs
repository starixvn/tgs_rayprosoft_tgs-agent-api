using System.Text.Json.Serialization;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Models.Commons
{
    public record DreamGameResponse
    {
        [JsonPropertyName("codeId")]
        public int CodeId { get; init; }
        [JsonPropertyName("token")]
        public string Token { get; init; }
    }
}
