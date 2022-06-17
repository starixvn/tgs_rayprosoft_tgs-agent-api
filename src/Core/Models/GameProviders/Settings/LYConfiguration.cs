using TGS.Core.Models.GameProviders.Settings.LY;

namespace TGS.Core.Models.GameProviders.Settings;

/// <summary>
/// LY 設定檔案
/// </summary>
public record LYConfiguration : PluggableConfigurationBase
{
    /// <summary>
    /// The amount maximum
    /// </summary>
    public decimal AmountMaximum { get; set; }

    /// <summary>
    /// The amount format
    /// </summary>
    public string AmountFormat { get; set; }

    /// <summary>
    /// The brand merchants
    /// </summary>
    public LYBrandMerchant[] BrandMerchants { get; set; }
}
