using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Jili.Incoming;

public record JiliBaseResponse
{
    [JsonPropertyName("errorCode")]
    public int ErrorCode { get; init; }

    [JsonPropertyName("message")]
    public string ErrorMessage { get; init; }
}
