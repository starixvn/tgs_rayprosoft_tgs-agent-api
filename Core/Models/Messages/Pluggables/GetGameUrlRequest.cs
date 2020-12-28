// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;
using TGS.Core.Models.Tokens;

namespace TGS.Core.Models.Messages.Pluggables
{
    public record GetGameUrlRequest
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