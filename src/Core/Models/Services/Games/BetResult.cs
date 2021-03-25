using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Games
{
    public record BetResult
    {
        public decimal Balance { get; init; }
        public string CurrencyCode { get; init; }
        public string GameProviderCurrencyCode { get; init; }
        public TransactionResult[] Transactions { get; init; }
        public TgsApiError Error { get; init; }
    }
}