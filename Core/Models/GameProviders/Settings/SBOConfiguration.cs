namespace TGS.Core.Models.GameProviders.Settings
{
    public record SBOConfiguration : PluggableConfigurationBase
    {
        public string CompanyKey { get; init; }
    }
}