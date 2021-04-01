using System;

namespace TGS.Services.Report.Application.Models.Agent
{
    public class AgentDividendDetailModel
    {
        public Guid? id { get; set; }
        public DateTimeOffset SettlementDate { get; set; }
        public DateTimeOffset SettlementDateFrom { get; set; }
        public DateTimeOffset SettlementDateTo { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal BetAmount { get; set; }
        public decimal Turnover { get; set; }
        public decimal WinLossAmount { get; set; }
        public decimal NetProfit { get; set; }
        public decimal AgentDividendAmount { get; set; }
    }
}