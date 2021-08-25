namespace TGS.Core.Models.GameProviders.Settings.WMCasino
{
    public record WMCasinoBrandMerchant
    {
        public string BrandCode { get; init; }

        public WMCasinoAgent[] Agents { get; init; }
    }
}