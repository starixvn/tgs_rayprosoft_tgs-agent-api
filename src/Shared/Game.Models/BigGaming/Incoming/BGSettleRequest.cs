using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.BigGaming.Common;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BGSettleRequest : BGSignParamsRequest
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        [JsonPropertyName("loginId")]
        public string LoginId { get; set; }
        [JsonPropertyName("amount")]
        public string Amount { get; set; }
        [JsonPropertyName("moduleId")]
        public int ModuleId { get; set; }
        [JsonPropertyName("tranId")]
        public string TranId { get; set; }
        [JsonPropertyName("recalc")]
        public string Recalc { get; set; }
        [JsonPropertyName("gameResult")]
        public string GameResult { get; set; }
        [JsonPropertyName("orders")]
        public BGSettleOrderRequest[] orders { get; set; }
    }
}
