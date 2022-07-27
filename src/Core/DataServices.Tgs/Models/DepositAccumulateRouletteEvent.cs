using System;

namespace TGS.Core.DataServices.Tgs.Models;

public class DepositAccumulateRouletteEvent
{
    public Guid BrandId { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime DistributeTime { get; set; }

    public Guid Id { get; set; }

    public string PlayerCode { get; set; }

    public Guid PromotionId { get; set; }

    public int RouletteCount { get; set; }

    public decimal SettingDepositAmount { get; set; }

    public decimal TotalDepositAmount { get; set; }
}
