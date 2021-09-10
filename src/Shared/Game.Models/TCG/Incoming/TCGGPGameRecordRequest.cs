using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TCG.Incoming
{
    public record TCGGPGameRecordRequest
    {
        [JsonPropertyName("subGP")]
        public string SubGP { get; init; }
        [JsonPropertyName("merchantCode")]
        public string MerchantCode { get; init; }
        [JsonPropertyName("isDesignation")]
        public bool IsDesignation { get; init; }
        [JsonPropertyName("lastGameRecorDirectory")]
        public string LastGameRecorDirectory { get; init; }
        [JsonPropertyName("lastGameRecorFile")]
        public string LastGameRecorFile { get; init; }
    }
}
