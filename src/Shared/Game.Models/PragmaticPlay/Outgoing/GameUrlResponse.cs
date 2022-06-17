using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Outgoing;

public record GameUrlResponse : ErrorResponse
{
    [JsonPropertyName("gameURL")]
    public string Url { get; init; }
}