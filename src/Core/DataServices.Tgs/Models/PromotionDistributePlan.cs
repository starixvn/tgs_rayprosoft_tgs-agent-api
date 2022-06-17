#nullable enable

using System;

namespace TGS.Core.DataServices.Tgs.Models;

public class PromotionDistributePlan
{
    public Guid BrandId { get; set; }

    public long ConditionHash { get; set; }

    public string? Detail { get; set; }

    public Guid Id { get; set; }

    public Guid PromotionId { get; set; }
}
