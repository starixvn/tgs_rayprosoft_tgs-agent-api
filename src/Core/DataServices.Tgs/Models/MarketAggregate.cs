using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class MarketAggregate
    {
        public int Id { get; set; }
        public Guid BrandId { get; set; }
        public DateTimeOffset ReportDate { get; set; }
        public int RegisteredCount { get; set; }
        public int FirstFundInCount { get; set; }
        public decimal DepositAmount { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal BetAmount { get; set; }
        public decimal Turnover { get; set; }
        public decimal WinLossAmount { get; set; }
        public decimal NetProfit { get; set; }
    }
}