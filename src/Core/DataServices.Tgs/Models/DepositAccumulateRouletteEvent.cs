using System;

namespace TGS.Core.DataServices.Tgs.Models;

public class DepositAccumulateRouletteEvent
{
    public Guid BrandId { get; set; }

    public DateOnly Date { get; set; }

    public string PlayerCode { get; set; }

    public Guid PromotionId { get; set; }

    public int RouletteCount { get; set; }

    public decimal TotalDepositAmount { get; set; }
}
