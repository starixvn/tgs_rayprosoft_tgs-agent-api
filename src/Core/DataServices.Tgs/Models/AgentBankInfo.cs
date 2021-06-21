using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class AgentBankInfo
    {
        public Guid AgentId { get; set; }

        public string BankType { get; set; }

        public virtual Brand Brand { get; set; }

        public Guid BrandId { get; set; }

        public string CreatedByUsername { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public string Detail { get; set; }

        public Guid Id { get; set; }

        public virtual PaymentChannel PaymentChannel { get; set; }

        public Guid PaymentChannelId { get; set; }

        public virtual Player Player { get; set; }

        public string UpdatedByUsername { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }
    }
}
