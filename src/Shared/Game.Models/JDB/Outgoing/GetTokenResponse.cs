// unset

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.JDB.Outgoing;

public record GetTokenResponse : BaseResponse
{
    [JsonPropertyName("path")]
    public string Path { get; init; }
}
