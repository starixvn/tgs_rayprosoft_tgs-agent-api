using System;
using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record WithdrawFormViewModel
    {
        public Guid Id { get; init; }

        public FlowState Status { get; init; }

        public decimal Amount { get; init; }
        [JsonPropertyName("createon")]
        public DateTimeOffset CreatedOn { get; init; }
        [JsonPropertyName("updateon")]
        public DateTimeOffset UpdatedOn { get; init; }
    }
}