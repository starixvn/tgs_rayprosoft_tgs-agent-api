using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record EvolutionConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }
        public string Ua2Token { get; set; }
        public string GameHistoryApiToken { get; set; }
        public string ExternalLobbyApiToken { get; set; }
        public Uri Ua2AuthenticationUrl { get; set; }
        public string OwAuthToken { get; set; }
    }
}
