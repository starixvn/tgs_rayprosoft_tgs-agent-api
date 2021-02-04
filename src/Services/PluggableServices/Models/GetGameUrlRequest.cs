// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;
using MediatR;
using TGS.Core.Models.Tokens;

namespace TGS.Services.PluggableServices.Models
{
    public record GetGameUrlRequest : IRequest<GetGameUrlResponse>
    {
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }

        [JsonPropertyName("gid")]
        public int GameId { get; init; }

        [JsonPropertyName("token")]
        public PlayerTokenData Token { get; init; }

        [JsonPropertyName("language")]
        public string Language { get; init; }
    }
}