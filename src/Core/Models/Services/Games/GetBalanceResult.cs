namespace TGS.Core.Models.Services.Games
{
    public record GetBalanceResult
    {
        public string Username { get; init; }
        public decimal Balance { get; init; }
        public string GameProviderCurrencyCode { get; init; }
    }
}