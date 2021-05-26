using System;

namespace TGS.Services.Report.Application.Models.Market
{
    public class MarketAggregateModel
    {
        public Guid BrandId { get; set; }
        public DateTimeOffset ReportDate { get; set; }
        public int RegisteredCount { get; set; }
        public int FirstFundInCount { get; set; }
        public decimal DepositAmount { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal RebateAmount { get; set; }
        public decimal DividendAmount { get; set; }
        public decimal PromotionAmount { get; set; }
        public decimal BetAmount { get; set; }
        public decimal Turnover { get; set; }
        public decimal WinLossAmount { get; set; }
        public decimal NetProfit { get; set; }
    }
}