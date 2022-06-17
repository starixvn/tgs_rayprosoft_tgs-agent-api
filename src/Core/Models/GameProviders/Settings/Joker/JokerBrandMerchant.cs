using System.Collections.Generic;

namespace TGS.Core.Models.GameProviders.Settings.Joker;

public record JokerBrandMerchant
{
    public string BrandCode { get; init; } = default!;

    public IEnumerable<JokerAgent> Agents { get; init; } = default!;
}
