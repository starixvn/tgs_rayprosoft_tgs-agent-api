using System.Text.Json.Serialization;

namespace TGS.Apis.TgsApi.GameModels.SBOBET.Incoming
{
    public record CancelBetRequest : BaseRequest
    {
        [JsonPropertyName("transferCode")]
        public string ExternalTransactionId { get; set; }

        [JsonPropertyName("transactionId")]
        public string ExternalRoundId { get; set; }

        [JsonPropertyName("IsCancelAll")]
        public bool IsCancelAll { get; set; }
    }
}