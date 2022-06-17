using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.LY.Commons;

namespace TGS.Shared.Game.Models.LY.Outgoing;

public record LYFnudRequest : LYBaseRequest
{
    [JsonPropertyName("account")]
    public string PlayerCode { get; set; }

    [JsonPropertyName("money")]
    [Column(TypeName = "decimal(12, 2)")]
    public decimal Amount { get; set; }

    [JsonPropertyName("orderid")]
    public string OrderId { get; set; }
}
