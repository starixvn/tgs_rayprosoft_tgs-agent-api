using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Deposit
{
    public record DepositFormViewModel
    {
        public Guid Id { get; init; }

        public string PlayerCode { get; init; }

        public string UserName { get; init; }

        public Guid BrandId { get; init; }

        public Guid BrandPaymentId { get; init; }

        public decimal Amount { get; init; }

        public string Remark { get; init; }

        public DateTimeOffset CreateOn { get; init; }

        public DateTimeOffset UpdateOn { get; init; }
    }
}
