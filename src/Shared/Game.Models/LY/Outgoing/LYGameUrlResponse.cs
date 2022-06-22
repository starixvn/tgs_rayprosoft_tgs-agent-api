using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.LY.Commons;

namespace TGS.Shared.Game.Models.LY.Outgoing;

public record LYGameUrlResponse : LYBaseDataResponse
{
    [JsonPropertyName("url")]
    public string Url { get; init; }
}
