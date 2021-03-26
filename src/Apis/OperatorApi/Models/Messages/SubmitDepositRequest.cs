using System;
using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record SubmitDepositRequest
    {
        [JsonPropertyName("account")]
        public string BankAccount { get; init; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("paymentid")]
        public Guid BrandPaymentId { get; init; }
    }
}
