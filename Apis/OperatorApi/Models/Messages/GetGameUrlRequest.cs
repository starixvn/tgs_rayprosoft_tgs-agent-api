using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record GetGameUrlRequest : IRequest<GetGameUrlResponse>
    {
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }

        [JsonPropertyName("gid")]
        public int GameId { get; init; }
        
        [JsonPropertyName("language")]
        public string LanguageCode { get; init; }
    }
}