using System;
using System.Text.Json.Serialization;
using TGS.Apis.TgsApi.GameModels.SBOBET.Enums;

namespace TGS.Apis.TgsApi.GameModels.SBOBET.Incoming
{
    public record SettleBetRequest : BaseRequest
    {
        [JsonPropertyName("transferCode")]
        public string ExternalTransactionId { get; init; }

        [JsonPropertyName("winloss")]
        public decimal WinLoss { get; init; }

        [JsonPropertyName("resultType")]
        public ResultType Result { get; init; }

        [JsonPropertyName("resultTime")]
        public DateTimeOffset SettleTime { get; init; }
    }
}