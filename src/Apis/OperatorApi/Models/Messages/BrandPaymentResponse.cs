using System;
using System.Text.Json;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public class BrandPaymentResponse
    {
        public Guid Id { get; init; }

        public string ChannelName { get; init; }

        public ValueRange<decimal> TransferLimit { get; init; }

        public JsonDocument ExtendedData { get; init; }

        public Guid PaymentId { get; init; }

        public string PaymentCode { get; init; }

        public Guid MethodId { get; init; }

        public string MethodCode { get; init; }
    }
}
