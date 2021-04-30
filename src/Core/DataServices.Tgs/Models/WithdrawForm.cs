using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class WithdrawForm
    {
        public Guid Id { get; set; }

        public string PlayerCode { get; set; }

        public Guid BrandId { get; set; }

        public decimal Amount { get; set; }

        public Guid? BrandPaymentId { get; set; }

        public int Status { get; set; }

        public string Remark { get; set; }

        public string BankInfo { get; set; }

        public string ApplyInfo { get; set; }

        public string PaymentReturn { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public virtual Player Player { get; set; }

        public virtual Brand Brand { get; set; }

        public virtual BrandPayment BrandPayment { get; set; }
    }
}
