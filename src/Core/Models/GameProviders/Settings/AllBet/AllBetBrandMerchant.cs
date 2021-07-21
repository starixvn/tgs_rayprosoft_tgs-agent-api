namespace TGS.Core.Models.GameProviders.Settings.AllBet
{
    public record AllBetBrandMerchant
    {
        public string BrandCode { get; init; }

        public AllBetAgent[] Agents { get; init; }
    }
}