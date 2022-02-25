namespace TGS.Apis.BackendOffice.Models.BrandGameProvider
{
    public record GameProviderRoundTotalReportModel
    {
        public string CurrencyCode { get; init; } = default!;

        public int TransactionCount { get; init; }

        public decimal BetAmount { get; init; }

        public decimal ValidBetAmount { get; init; }

        public decimal OriginalTurnover { get; init; }

        public decimal CompanyWinLoss { get; init; }
    }
}