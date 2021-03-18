using System;
using System.Text.Json.Serialization;
using MediatR;
using TGS.Shared.Game.Models.SBOBET.Enums;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record SettleBetRequest : TransactionRequest, IRequest<SettleBetResponse>
    {
        [JsonPropertyName("winloss")]
        public decimal WinLoss { get; init; }

        [JsonPropertyName("resultType")]
        public ResultType Result { get; init; }

        [JsonPropertyName("resultTime")]
        public DateTimeOffset SettleTime { get; init; }
    }
}