using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using TGS.Core.Models.Enums;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record GetLobbyGameUrlRequest : IRequest<GetGameUrlResponse>
    {
        [Required]
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }

        [Required]
        [JsonPropertyName("gtype")]
        public GameType GameType { get; init; }

        [Required]
        [JsonPropertyName("language")]
        public string LanguageCode { get; init; }

        public JsonDocument Extends { get; set; }
    }
}