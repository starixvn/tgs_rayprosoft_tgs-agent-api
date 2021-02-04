using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record CancelBetRequest : BaseRequest, IRequest<CancelBetResponse>
    {
        [JsonPropertyName("transferCode")]
        public string ExternalTransactionId { get; set; }

        [JsonPropertyName("transactionId")]
        public string ExternalRoundId { get; set; }

        [JsonPropertyName("IsCancelAll")]
        public bool IsCancelAll { get; set; }
    }
}