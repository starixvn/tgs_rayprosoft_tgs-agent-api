namespace TGS.Services.GameOperations.Models.Data
{
    public record BetValuesResult
    {
        public decimal BetAmount { get; init; }
        public decimal TieBetAdjustment { get; init; }
        public decimal Turnover { get; init; }
        public decimal WinLoss { get; init; }
        public decimal UnsettledBets { get; init; }
    }
}
