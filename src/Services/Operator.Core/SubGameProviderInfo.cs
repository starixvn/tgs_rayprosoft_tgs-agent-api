using System;

namespace TGS.Services.Operator
{
    public record SubGameProviderInfo(Guid GameProviderId, string Code, string Name,
        string Description, string Localization);
}