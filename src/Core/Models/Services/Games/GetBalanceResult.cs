namespace TGS.Core.Models.Services.Games
{
    public record GetBalanceResult
    {
        public decimal Balance { get; init; }
        public string CurrencyCode { get; init; }
    }
}