using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models.Messages
{
    public class BankInfoResponse
    {
        [JsonPropertyName("banktype")]
        public string BankType { get; init; }

        [JsonPropertyName("detail")]
        public JsonDocument Detail { get; init; }

        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("paymentchannelid")]
        public Guid PaymentChannelId { get; init; }
    }
}
