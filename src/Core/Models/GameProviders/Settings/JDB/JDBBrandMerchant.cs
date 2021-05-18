using System;

namespace TGS.Core.Models.GameProviders.Settings.JDB
{
    public record JDBBrandMerchant
    {
        public string BrandCode { get; init; }

        public string AESIV { get; set; }

        public string AESKEY { get; set; }

        public JDBAgentMapper[] AgentMappers { get; set; }
        
        public bool Status { get; set; }
    }
}
