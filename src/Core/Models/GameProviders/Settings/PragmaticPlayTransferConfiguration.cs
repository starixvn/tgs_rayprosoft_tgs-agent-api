using System;

namespace TGS.Core.Models.GameProviders.Settings;

public record PragmaticPlayTransferConfiguration : PluggableConfigurationBase
{
    public Uri ApiUrl { get; init; }
    public string ProviderId { get; init; }
    public string SecretKey { get; init; }
    public string SecureLogin { get; init; }
}
