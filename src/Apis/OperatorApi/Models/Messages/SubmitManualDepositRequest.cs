using System;
using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record SubmitManualDepositRequest
    {
        [JsonPropertyName("account")]
        public string BankAccount { get; init; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("paymentid")]
        public Guid BrandPaymentId { get; init; }

        [JsonPropertyName("promotionid")]
        public Guid? PromotionId { get; init; }
    }
}
