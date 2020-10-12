using System.Text.Json.Serialization;
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;

namespace TGS.Apis.TgsApi.GameModels
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