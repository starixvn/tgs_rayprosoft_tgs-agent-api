using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Incoming
{
    public record TransactionResponse : BaseResponse
    {
        [JsonPropertyName("OperatorTransactionId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid? InternalTransactionId { get; init; }

        [JsonPropertyName("TransactionId")]
        public string ExternalTransactionId { get; init; }
    }
}