using System;
using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public class SubmitBankDepositRequest
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("paymentid")]
        public Guid BrandPaymentId { get; init; }
    }
}
