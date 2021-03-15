namespace TGS.Services.GameOperations.Models.Data
{
    public record BetValuesResult
    {
        public decimal BetAmount { get; init; }
        public decimal ValidBetAmount { get; init; }
        public decimal SettleAmount { get; init; }
        public decimal TieBetAdjustment { get; init; }
        public decimal Turnover { get; init; }

        /// <summary>
        ///     Company WinLoss
        /// </summary>
        public decimal WinLoss { get; init; }

        public decimal UnsettledBets { get; init; }
    }
}