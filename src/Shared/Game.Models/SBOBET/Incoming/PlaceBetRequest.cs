using System;
using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record PlaceBetRequest : TransactionRequest, IRequest<PlaceBetResponse>
    {
        [JsonPropertyName("transactionId")]
        public string ExternalRoundId { get; init; }

        [JsonPropertyName("betTime")]
        public DateTimeOffset BetTime { get; init; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }
    }
}