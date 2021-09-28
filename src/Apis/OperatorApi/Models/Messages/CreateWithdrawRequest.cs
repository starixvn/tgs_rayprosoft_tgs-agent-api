using System;
using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public class CreateWithdrawRequest
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("bankinfoid")]
        public Guid BankInfoId { get; init; }
    }
}
