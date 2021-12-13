using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class BrandAllAgentGameTypeCondition
    {
        public AllAgentRebateType RebateType { get; set; }

        public int Rank { get; set; }

        public Guid BrandId { get; set; }

        public decimal NetProfit { get; set; }

        public int ActiveNumber { get; set; }

        public decimal DividendPercentage { get; set; }
    }
}