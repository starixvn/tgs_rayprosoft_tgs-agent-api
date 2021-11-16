namespace TGS.Core.Models.GameProviders.Settings
{
    public record BetConstructConfiguration : PluggableConfigurationBase
    {
        public string SharedKey { get; set; }

        public string ParnterId { get; set; }
    }
}
