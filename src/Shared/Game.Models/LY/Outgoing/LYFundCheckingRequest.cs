using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.LY.Commons;

namespace TGS.Shared.Game.Models.LY.Outgoing;

public record LYFundCheckingRequest : LYBaseRequest
{
    [JsonPropertyName("orderid")]
    public string OrderId { get; init; }
}
