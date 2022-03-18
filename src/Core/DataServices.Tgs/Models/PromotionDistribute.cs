#nullable enable

using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PromotionDistribute
    {
        public decimal BonusAmount { get; set; }

        public Guid BrandId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public Guid Id { get; set; }

        public Guid? PlanId { get; set; }

        public string PlayerCode { get; set; } = default!;

        public Guid PromotionId { get; set; }

        public string PromotionType { get; set; } = default!;

        public string? Distributor { get; set; }

        public DateTimeOffset? SentOn { get; set; }
    }
}
