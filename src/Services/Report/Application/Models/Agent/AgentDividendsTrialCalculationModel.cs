using System;

namespace TGS.Services.Report.Application.Models.Agent
{
    public class AgentDividendsTrialCalculationModel
    {
        public string AgentUsername { get; set; }
        public Guid AgentId { get; set; }
        public string BrandCode { get; set; }
        public Guid BrandId { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public decimal AgentDividendPercentage { get; set; }
        public decimal AgentDividendAmount { get; set; }
        public decimal NegativeDividendAmount { get; set; }
        public bool AutoSend { get; set; }
    }
}