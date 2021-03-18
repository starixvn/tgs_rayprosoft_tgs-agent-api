using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record CancelBetRequest : TransactionRequest, IRequest<CancelBetResponse>
    {
        [JsonPropertyName("transactionId")]
        public string ExternalRoundId { get; init; }

        [JsonPropertyName("IsCancelAll")]
        public bool IsCancelAll { get; init; }
    }
}