namespace TGS.Core.Models.Settings
{
    public record CurrencyNetProfit
    {
        public string CurrencyCode { get; init; }
        public decimal NetProfit { get; init; }
        public decimal[] SendPercentages { get; init; }
    }
}