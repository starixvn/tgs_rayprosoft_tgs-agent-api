using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record PlaceBetResponse : BaseResponse
    {
        [JsonPropertyName("refId")]
        public long TransactionId { get; init; }

        [JsonPropertyName("licenseeTxId")]
        public long ExternalRoundId { get; init; }
    }
}