using System;
using TGS.Core.Models.Enums;

namespace TGS.Clients.Promotion.Client.Models.Promotion
{
    public class UpdatePromotionRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public SwitchStatus Status { get; set; }
        public string PromotionType { get; set; }
        public decimal BonusAmountLimit { get; set; }
        public bool Unlimited { get; set; }
        public GameType? GameType { get; set; }
        public decimal Percentage { get; set; }
        public decimal TotalAmountLimit { get; set; }
        public decimal TurnoverMultiple { get; set; }
        public Guid[] GameProviderIds { get; set; }
        public int[] VipLevels { get; set; }
        public string EditorUsername { get; set; }
    }
}