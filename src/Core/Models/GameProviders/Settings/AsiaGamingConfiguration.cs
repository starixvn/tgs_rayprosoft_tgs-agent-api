using System;
using System.Collections.Generic;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record AsiaGamingConfiguration : PluggableConfigurationBase
    {
        public string AgentCode { get; init; }
        public string ProductionId { get; init; }
        public string DesKey { get; init; }
        public string Md5Key { get; init; }
        public Uri LoginApiUrl { get; init; }
        public Uri ApiUrl { get; init; }
        public OppositeBetSetting[] OppositeBets { get; init; }
        public string[] LockGameTypes { get; init; }
        public AsiaGamingGameHistoryType[] GameHistoryTypes { get; init; }
    }

    public record AsiaGamingGameHistoryType
    {
        public string[] GameTypes { get; init; }
        public IDictionary<string, string> Descriptions { get; init; }
    }
}