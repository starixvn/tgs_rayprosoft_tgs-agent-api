using System.Text.Json.Serialization;

namespace TGS.Apis.TgsApi.GameModels.SBOBET.Incoming
{
    public record RollbackRequest : BaseRequest
    {
        [JsonPropertyName("transferCode")]
        public string ExternalTransactionId { get; set; }
    }
}