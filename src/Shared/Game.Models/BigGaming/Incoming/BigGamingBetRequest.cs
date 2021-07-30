using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.BigGaming.Commons;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BigGamingBetRequest : BigGamingParamsSignLoginIdRequest
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        [JsonPropertyName("amount")]
        public string Amount { get; set; }
        [JsonPropertyName("moduleId")]
        public int ModuleId { get; set; }
        [JsonPropertyName("tranId")]
        public string TranId { get; set; }
        [JsonPropertyName("orders")]
        public BigGamingBetOrderRequest[] Orders { get; set; }
    }
}
