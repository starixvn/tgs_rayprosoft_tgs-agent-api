// unset

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.JDB.Incoming
{
    public record GetTokenResponse : BaseResponse
    {
        [JsonPropertyName("path")]
        public string Path { get; init; }
    }
}