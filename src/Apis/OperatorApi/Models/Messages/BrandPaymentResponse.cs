using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record BrandPaymentResponse
    {
        [JsonPropertyName("banktype")]
        public string BankType { get; init; }

        [JsonPropertyName("channelid")]
        public Guid ChannelId { get; init; }

        [JsonPropertyName("channelname")]
        public string ChannelName { get; init; }

        [JsonPropertyName("channelcode")]
        public string ChannelCode { get; init; }

        [JsonPropertyName("extended")]
        public JsonDocument ExtendedData { get; init; }

        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("methodcode")]
        public string MethodCode { get; init; }

        [JsonPropertyName("methodid")]
        public Guid MethodId { get; init; }

        [JsonPropertyName("paymentcode")]
        public string PaymentCode { get; init; }

        [JsonPropertyName("paymentid")]
        public Guid PaymentId { get; init; }

        [JsonPropertyName("transferlimit")]
        public ValueRange<decimal> TransferLimit { get; init; }
    }
}
