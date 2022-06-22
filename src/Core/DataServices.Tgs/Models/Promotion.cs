using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models;

public class Promotion
{
    public decimal AmountUsed { get; set; }

    public Guid? AmountUsedSnapshotId { get; set; }

    public bool AutoSentPlayerBonus { get; set; }

    public decimal BonusAmountLimit { get; set; }

    public virtual Brand Brand { get; set; }

    public Guid BrandId { get; set; }

    public string CampaignDetail { get; set; }

    public string Content { get; set; }

    public string CreatedByUsername { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public string DesktopBanner { get; set; }

    public int DisplayOrder { get; set; }

    public DateTimeOffset From { get; set; }

    public GameType? GameType { get; set; }

    public Guid Id { get; set; }

    public string Localization { get; set; }

    public string MobileBanner { get; set; }

    public string Name { get; set; }

    public decimal Percentage { get; set; }

    public virtual ICollection<PromotionGameProvider> PromotionGameProviders { get; set; }

    public virtual ICollection<PromotionParticipant> PromotionParticipants { get; set; }

    public string PromotionType { get; set; }

    public virtual ICollection<PromotionVipLevel> PromotionVipLevels { get; set; }

    public string SettlementIntervalType { get; set; }

    public PromotionStatus Status { get; set; }

    public string SubSubject { get; set; }

    public string Terms { get; set; }

    public DateTimeOffset To { get; set; }

    public decimal TotalAmountLimit { get; set; }

    public decimal TurnoverMultiple { get; set; }

    public bool Unlimited { get; set; }

    public string UpdatedByUsername { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }
}
