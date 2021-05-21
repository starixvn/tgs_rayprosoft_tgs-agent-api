using System;
using TGS.Apis.BackendOffice.Controllers.Messages.Deposit;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Withdraw
{
    public record WithdrawFormViewModel
    {
        public decimal Amount { get; init; }

        public Guid BrandId { get; init; }

        public Guid? BrandPaymentId { get; init; }

        public string ChannelName { get; init; }

        public DateTimeOffset CreatedOn { get; init; }

        public Guid Id { get; init; }

        public Guid? PaymentMethodId { get; init; }

        public string PlayerCode { get; init; }

        public string Remark { get; init; }

        public FlowState Status { get; init; }

        public DateTimeOffset UpdatedOn { get; init; }

        public string UserName { get; init; }
    }
}
