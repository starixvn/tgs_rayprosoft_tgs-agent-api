using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public class GetGameUrlRequest
    {
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }

        [JsonPropertyName("gid")]
        public int GameId { get; init; }
        
        [JsonPropertyName("language")]
        public string LanguageCode { get; init; }
    }
}