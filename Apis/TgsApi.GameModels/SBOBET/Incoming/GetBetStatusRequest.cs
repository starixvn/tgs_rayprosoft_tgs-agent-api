using System.Text.Json.Serialization;

namespace TGS.Apis.TgsApi.GameModels.SBOBET.Incoming
{
    public record GetBetStatusRequest : BaseRequest
    {
        [JsonPropertyName("transferCode")]
        public string ExternalTransactionId { get; set; }

        [JsonPropertyName("transactionId")]
        public string ExternalRoundId { get; set; }
    }
}