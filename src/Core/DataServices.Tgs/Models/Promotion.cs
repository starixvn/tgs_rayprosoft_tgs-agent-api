using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class Promotion
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public SwitchStatus Status { get; set; }
        public bool AutoSentPlayerBonus { get; set; }
        public Guid BrandId { get; set; }
        public virtual Brand Brand { get; set; }
        public string PromotionType { get; set; }
        public string SettlementIntervalType { get; set; }
        public decimal TotalAmountLimit { get; set; }
        public decimal AmountUsed { get; set; }
        public bool Unlimited { get; set; }
        public GameType? GameType { get; set; }
        public decimal Percentage { get; set; }
        public decimal BonusAmountLimit { get; set; }
        public decimal TurnoverMultiple { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
        public string Subject { get; set; }
        public string SubSubject { get; set; }
        public string Content { get; set; }
        public string Terms { get; set; }
        public string Localization { get; set; }
        public virtual ICollection<PromotionGameProvider> PromotionGameProviders { get; set; }
        public virtual ICollection<PromotionVipLevel> PromotionVipLevels { get; set; }
        public virtual ICollection<PromotionParticipant> PromotionParticipants { get; set; }
        public virtual ICollection<PromotionImage> PromotionImages { get; set; }
    }
}