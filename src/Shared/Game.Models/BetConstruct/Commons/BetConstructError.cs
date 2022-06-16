using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BetConstruct.Commons;

public record BetConstructError
{
    [JsonPropertyName("code")]
    public string Code { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
}
