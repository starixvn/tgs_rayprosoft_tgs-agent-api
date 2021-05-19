using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.BigGaming.Common;
using TGS.Shared.Game.Models.BigGaming.Enums;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BigGamingSettleRequest : BigGamingSignParamsRequest
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        [JsonPropertyName("loginId")]
        public string LoginId { get; set; }
        [JsonPropertyName("amount")]
        public string Amount { get; set; }
        [JsonPropertyName("moduleId")]
        public BigGamingModuleId ModuleId { get; set; }
        [JsonPropertyName("tranId")]
        public string TranId { get; set; }
        [JsonPropertyName("gameResult")]
        public string GameResult { get; set; }
        [JsonPropertyName("recalc")]
        public string Recalc { get; set; }
        [JsonPropertyName("orders")]
        public BigGamingSettleOrderRequest[] Orders { get; set; }
    }
}
