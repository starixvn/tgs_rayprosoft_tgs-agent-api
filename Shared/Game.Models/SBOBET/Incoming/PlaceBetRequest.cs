using System;
using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record PlaceBetRequest : BaseRequest, IRequest<PlaceBetResponse>
    {
        [JsonPropertyName("transferCode")]
        public string ExternalTransactionId { get; set; }

        [JsonPropertyName("transactionId")]
        public string ExternalRoundId { get; set; }

        [JsonPropertyName("betTime")]
        public DateTimeOffset BetTime { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }
}