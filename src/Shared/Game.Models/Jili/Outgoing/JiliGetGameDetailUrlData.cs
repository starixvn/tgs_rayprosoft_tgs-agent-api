using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Jili.Outgoing;

public record JiliGetGameDetailUrlData
{
    [JsonPropertyName("Url")]
    public string Url { get; init; }
}
