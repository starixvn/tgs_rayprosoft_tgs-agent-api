namespace TGS.Core.Models.GameProviders.Settings
{
    public record SabaConfiguration : PluggableConfigurationBase
    {
        public string SecretKey { get; init; }
    }
}