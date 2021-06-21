using System;
using TGS.Core.Models.Enums;

namespace TGS.Clients.Promotion.Client.Models.Promotion
{
    public class PromotionModel
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public SwitchStatus Status { get; set; }
        public string PromotionType { get; set; }
        public decimal BonusAmountLimit { get; set; }
        public decimal AmountUsed { get; set; }
        public bool AutoSentPlayerBonus { get; set; }
        public bool Unlimited { get; set; }
        public GameType? GameType { get; set; }
        public decimal Percentage { get; set; }
        public decimal TotalAmountLimit { get; set; }
        public decimal TurnoverMultiple { get; set; }
        public Guid[] GameProviderIds { get; set; }
        public int[] VipLevels { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
        public string Subject { get; set; }
        public string SubSubject { get; set; }
        public string Content { get; set; }
        public string Terms { get; set; }
        public string Localization { get; set; }
        public PromotionImageModel[] Images { get; set; }
    }
}