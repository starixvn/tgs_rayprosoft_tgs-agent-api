using System;

namespace TGS.Services.Report.Application.Models.Agent
{
    public class AgentDividendDetailModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public DateTimeOffset SettlementDate { get; set; }
        public DateTimeOffset SettlementDateFrom { get; set; }
        public DateTimeOffset SettlementDateTo { get; set; }
        public decimal LiveDealerWinLoss { get; set; }
        public decimal SlotWinLoss { get; set; }
        public decimal LotteryWinLoss { get; set; }
        public decimal CardGameWinLoss { get; set; }
        public decimal SportsWinLoss { get; set; }
        public decimal VirtualSportsWinLoss { get; set; }
        public decimal WinLossAmount { get; set; }
        public decimal AgentDividendAmount { get; set; }
        public decimal AgentDividendPercentage { get; set; }
    }
}