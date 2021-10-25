namespace TGS.Services.Report.Application.Models.LicenseeMarket
{
    public class ExportLicenseeMarketsModel
    {
        public decimal AdjustmentAmount { get; set; }

        public decimal BetAmount { get; set; }

        public decimal BonusAmount { get; set; }

        public string BrandCode { get; set; }

        public string CurrencyCode { get; set; }

        public decimal DepositAmount { get; set; }

        public int FirstFundInCount { get; set; }

        public decimal NetProfit { get; set; }

        public int RegisteredCount { get; set; }

        public decimal Turnover { get; set; }

        public decimal WinLossAmount { get; set; }

        public decimal WithdrawalAmount { get; set; }
    }
}
