using System;
using TGS.Core.Models.GameProviders.Settings.JDB;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record JDBConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }
        public bool MoreGame { get; init; }
        
        public string AESIV { get; init; }

        public string AESKEY { get; init; }

        public string ClientDomain { get; init; }
        
        public JDBAgent[] Agents { get; init; }
    }
}