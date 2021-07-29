using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.BigGaming.Commons;
using TGS.Shared.Game.Models.BigGaming.Enums;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BigGamingOrderStatusRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("moduleId")]
        public BigGamingModuleId ModuleId { get; set; }
        [JsonPropertyName("tranId")]
        public string TranId { get; set; }
        [JsonPropertyName("orderIds")]
        public long[] OrderIds { get; set; }
        [JsonPropertyName("orderMap")]
        public BigGamingOrderStatusOrderMapRequest[] OrderMap { get; set; }
    }
}
