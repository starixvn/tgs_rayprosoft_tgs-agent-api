using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.BigGaming.Commons;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BigGamingTransferRequest : BigGamingParamsSignLoginIdRequest
    {
        [JsonPropertyName("amount")]
        public string Amount { get; set; }
        [JsonPropertyName("bizId")]
        public long BizId { get; set; }
    }
}
