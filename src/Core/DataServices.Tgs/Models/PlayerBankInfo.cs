using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerBankInfo
    {
        public Guid Id { get; set; }

        public string PlayerCode { get; set; }

        public Guid BrandId { get; set; }

        public Guid PaymentChannelId { get; set; }

        public string BankType { get; set; }

        public string Detail { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string CreatedByUsername { get; set; }

        public string UpdatedByUsername { get; set; }

        public virtual Player Player { get; set; }

        public virtual Brand Brand { get; set; }

        public virtual PaymentChannel PaymentChannel { get; set; }
    }
}
