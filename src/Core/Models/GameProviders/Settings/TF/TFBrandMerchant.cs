using System;

namespace TGS.Core.Models.GameProviders.Settings.TF;

public record TFBrandMerchant
{
    public Uri LaunchUrl { get; set; }
    public Uri ApiUrl { get; set; }
    public string BrandCurrency { get; set; }
    public string BrandCode { get; set; }
    public string PublicToken { get; set; }
    public string PrivateToken { get; set; }
    public string PartnerId { get; set; }
}
