using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.LY.Commons;

namespace TGS.Shared.Game.Models.LY.Outgoing;

public record LYGameUrlRequest : LYBaseRequest
{
    [JsonPropertyName("account")]
    public string PlayerCode { get; init; }

    [JsonPropertyName("money")]
    public decimal Amount { get; init; }

    [JsonPropertyName("orderid")]
    public string OrderId { get; init; }

    [JsonPropertyName("ip")]
    public string IP { get; init; }

    [JsonPropertyName("lineCode")]
    public string BrandsCode { get; init; }

    [JsonPropertyName("KindID")]
    public int ExternalID { get; init; }
}
