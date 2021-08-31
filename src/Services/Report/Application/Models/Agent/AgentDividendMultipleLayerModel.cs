using System;

namespace TGS.Services.Report.Application.Models.Agent
{
    public class AgentDividendMultipleLayerModel
    {
        public Guid AgentId { get; set; }
        public Guid DividendProviderAgentId { get; set; }
        public string Layer { get; set; }
        public decimal Dividend { get; set; }
    }
}