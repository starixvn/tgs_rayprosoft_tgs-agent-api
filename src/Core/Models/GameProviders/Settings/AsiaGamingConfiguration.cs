namespace TGS.Core.Models.GameProviders.Settings
{
    public record AsiaGamingConfiguration : PluggableConfigurationBase
    {
        public OppositeBetSetting[] OppositeBets { get; init; }
    }
}