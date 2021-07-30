using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.BigGaming.Commons;

namespace TGS.Shared.Game.Models.BigGaming.Outgoing
{
    public record BigGamingVideoOrderDetailRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("orderId")]
        public long OrderId { get; set; }
        [JsonPropertyName("locale")]
        public string Locale { get; set; }
    }
}
