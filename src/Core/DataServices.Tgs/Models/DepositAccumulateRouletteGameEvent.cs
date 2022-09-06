using System;

namespace TGS.Core.DataServices.Tgs.Models;

public class DepositAccumulateRouletteGameEvent
{
    public decimal Bonus { get; set; }

    public Guid BrandId { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateOnly Date { get; set; }

    public Guid Id { get; set; }

    public int ItemIndex { get; set; }

    public string PlayerCode { get; set; }

    public Guid PromotionId { get; set; }
}
