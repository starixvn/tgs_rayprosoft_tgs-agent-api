using System;

namespace TGS.Core.DataServices.Report.Models
{
    public class PromotionPlayer
    {
        public int Id { get; set; }
        public string PlayerCode { get; set; }
        public Guid PromotionBonusId { get; set; }
        public virtual PromotionBonus PromotionBonus { get; set; }
        public decimal BonusAmount { get; set; }
        public DateTimeOffset? SentOn { get; set; }
    }
}