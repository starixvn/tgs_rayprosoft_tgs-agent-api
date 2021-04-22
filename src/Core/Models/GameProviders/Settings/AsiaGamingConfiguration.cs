using System;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record AsiaGamingConfiguration : PluggableConfigurationBase
    {
        public string AgentCode { get; init; }
        public string ProductId { get; init; }
        public string DesKey { get; init; }
        public string Md5Key { get; init; }
        public Uri LoginApiUrl { get; init; }
        public Uri ApiUrl { get; init; }
        public OppositeBetSetting[] OppositeBets { get; init; }
        public string[] LockGameTypes { get; init; }
    }
}