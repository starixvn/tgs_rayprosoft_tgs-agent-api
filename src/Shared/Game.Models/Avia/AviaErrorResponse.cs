using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Avia;

public record AviaErrorResponse
{
    [JsonPropertyName("Error")]
    public string Error { get; init; }
}
