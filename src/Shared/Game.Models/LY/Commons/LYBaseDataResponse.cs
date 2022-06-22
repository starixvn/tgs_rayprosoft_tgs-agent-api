using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.LY.Commons;

public record LYBaseDataResponse
{
    [JsonPropertyName("code")]
    public int Code { get; init; }
}
