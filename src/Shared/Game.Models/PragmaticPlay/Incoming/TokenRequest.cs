using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record TokenRequest : BaseRequest
    {
        [JsonPropertyName("token")]
        public string Token { get; init; }
    }
}