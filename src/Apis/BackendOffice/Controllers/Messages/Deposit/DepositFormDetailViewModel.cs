using System;
using System.Text.Json;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Deposit
{
    public class DepositFormDetailViewModel
    {
        public Guid Id { get; init; }

        public string PlayerCode { get; init; }

        public string UserName { get; init; }

        public Guid BrandId { get; init; }

        public Guid BrandPaymentId { get; init; }

        public Guid PaymentMethodId { get; init; }

        public string ChannelName { get; init; }

        public decimal Amount { get; init; }

        public string Remark { get; init; }

        public JsonDocument ApplyInfo { get; init; }

        public JsonDocument PaymentReturn { get; init; }

        public DateTimeOffset CreateOn { get; init; }

        public DateTimeOffset UpdateOn { get; init; }
    }
}
