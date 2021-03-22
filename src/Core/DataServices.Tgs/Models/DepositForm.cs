using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class DepositForm
    {
        public Guid Id { get; set; }

        public string PlayerCode { get; set; }

        public Guid BrandId { get; set; }

        public string BankAccount { get; set; }

        public decimal Amount { get; set; }

        public Guid BrandPaymentId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}
