using System;

namespace TGS.Core.DataServices.Tgs.Models;

public class PlayerBonusAggregate
{
    public Guid BrandId { get; set; }

    public Guid? AgentId { get; set; }

    public string PlayerCode { get; set; }

    public int Hour { get; set; }

    public int Minute { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset ReportDate { get; set; }

    public decimal RebateAmount { get; set; }

    public decimal PromotionAmount { get; set; }

    public decimal UpgradeAmount { get; set; }

    public decimal SeasonAmount { get; set; }

    public decimal BirthdayAmount { get; set; }
}