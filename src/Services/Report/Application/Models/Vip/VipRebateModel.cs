using System;

namespace TGS.Services.Report.Application.Models.Vip
{
    public class VipRebateModel
    {
        public DateTimeOffset ReportDate { get; set; }
        public decimal LiveDealerAmount { get; set; }
        public decimal SlotAmount { get; set; }
        public decimal CardGameAmount { get; set; }
        public decimal SportsAmount { get; set; }
        public decimal VirtualSportsAmount { get; set; }
        public decimal LiveDealerValidBetAmount { get; set; }
        public decimal SlotValidBetAmount { get; set; }
        public decimal CardGameValidBetAmount { get; set; }
        public decimal SportsValidBetAmount { get; set; }
        public decimal VirtualSportsValidBetAmount { get; set; }
        public decimal LiveDealerRebate { get; set; }
        public decimal SlotRebate { get; set; }
        public decimal CardGameRebate { get; set; }
        public decimal SportsRebate { get; set; }
        public decimal VirtualSportsRebate { get; set; }
    }
}