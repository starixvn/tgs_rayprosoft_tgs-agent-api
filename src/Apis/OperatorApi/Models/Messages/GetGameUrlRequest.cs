using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record GetGameUrlRequest : IRequest<GetGameUrlResponse>
    {
        [Required]
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }

        [Required]
        [JsonPropertyName("gid")]
        public int GameId { get; init; }

        [Required]
        [JsonPropertyName("language")]
        public string LanguageCode { get; init; }

        [JsonPropertyName("extends")]
        public JsonDocument ExtendedData { get; set; }
    }
}