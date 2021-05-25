using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Models.GameProviders.Settings.SexyGaming;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record SexyGamingConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }
        public string AgentId { get; init; }
        public string Cert { get; init; }
        public SexyGamingBetLimit BetLimit { get; init; }
    }
}
