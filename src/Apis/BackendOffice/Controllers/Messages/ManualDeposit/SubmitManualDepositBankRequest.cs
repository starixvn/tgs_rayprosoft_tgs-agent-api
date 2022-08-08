using System;
using System.Text.Json.Serialization;

#nullable enable

namespace TGS.Apis.BackendOffice.Controllers.Messages.ManualDeposit
{
    public record SubmitManualDepositBankRequest
    {
        [JsonPropertyName("playercode")]
        public string PlayerCode { get; init; } = default!;
        [JsonPropertyName("brandid")]
        public Guid BrandId { get; init; }
        [JsonPropertyName("account")]
        public string BankAccount { get; init; } = default!;

        [JsonPropertyName("name")]
        public string? BankName { get; init; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("paymentid")]
        public Guid BrandPaymentId { get; init; }

        [JsonPropertyName("promotionid")]
        public Guid? PromotionId { get; init; }

        [JsonPropertyName("banktype")]
        public string BankType { get; init; } = default!;
        [JsonPropertyName("note")]
        public string? Note { get; init; }
    }
}
