using System;
using System.Text.Json;
using TGS.Core.Models.Enums;

namespace TGS.Services.Promotion.Models.Promotion
{
    public class PromotionModel
    {
        public decimal AmountUsed { get; set; }

        public bool AutoSentPlayerBonus { get; set; }

        public decimal BonusAmountLimit { get; set; }

        public Guid BrandId { get; set; }

        public string Content { get; set; } = default!;

        public string CreatedByUsername { get; set; } = default!;

        public DateTimeOffset CreatedOn { get; set; }

        public string DesktopBanner { get; set; } = default!;

        public DateTimeOffset From { get; set; }

        public Guid[] GameProviderIds { get; set; } = default!;

        public GameType? GameType { get; set; }

        public Guid Id { get; set; }

        public JsonDocument? Localization { get; set; }

        public string? LocalizationString { get; set; }

        public string MobileBanner { get; set; } = default!;

        public string Name { get; set; } = default!;

        public decimal Percentage { get; set; }

        public string PromotionType { get; set; } = default!;

        public SwitchStatus Status { get; set; }

        public string SubSubject { get; set; } = default!;

        public string Terms { get; set; } = default!;

        public DateTimeOffset To { get; set; }

        public decimal TotalAmountLimit { get; set; }

        public decimal TurnoverMultiple { get; set; }

        public bool Unlimited { get; set; }

        public string UpdatedByUsername { get; set; } = default!;

        public DateTimeOffset UpdatedOn { get; set; }

        public int[] VipLevels { get; set; } = default!;
    }
}
