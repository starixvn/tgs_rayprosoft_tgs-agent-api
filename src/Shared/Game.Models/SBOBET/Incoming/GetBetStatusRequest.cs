using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record GetBetStatusRequest : BaseRequest, IRequest<GetBetStatusResponse>
    {
        [JsonPropertyName("transferCode")]
        public string ExternalTransactionId { get; set; }

        [JsonPropertyName("transactionId")]
        public string ExternalRoundId { get; set; }
    }
}