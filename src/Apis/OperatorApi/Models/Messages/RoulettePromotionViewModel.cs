using System;

namespace TGS.Apis.OperatorApi.Models.Messages;

public class RoulettePromotionViewModel
{
    public Guid PromotionId { get; init; }

    public string PromotionType { get; init; }

    public int RouletteCount { get; init; }
}
