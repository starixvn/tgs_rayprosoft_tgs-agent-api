using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PaymentHandlingFee
    {
        public decimal Amount { get; set; }

        public Guid BrandId { get; set; }

        public decimal Fee { get; set; }

        public Guid FormId { get; set; }

        public Guid Id { get; set; }

        public DateTime RecordedOn { get; set; }

        public string Remark { get; set; }

        public string Type { get; set; }
    }
}
