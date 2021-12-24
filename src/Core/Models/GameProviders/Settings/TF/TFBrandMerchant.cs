namespace TGS.Core.Models.GameProviders.Settings.TF
{
    public record TFBrandMerchant
    {
        public string BrandCode { get; set; }
        public string PublicToken { get; set; }
        public string PrivateToken { get; set; }
        public string PartnerId { get; set; }
    }
}
