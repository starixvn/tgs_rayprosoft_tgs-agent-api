using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerIncreaseAmount
    {
        public Guid Id { get; set; }

        public Guid BrandId { get; set; }

        public string PlayerCode { get; set; }

        public PlayerIncreaseAmountType Type { get; set; }

        public decimal Amount { get; set; }

        public decimal BucketAmount { get; set; }

        public string Remark { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string CreatedByUsername { get; set; }

        public string UpdatedByUsername { get; set; }
    }
}