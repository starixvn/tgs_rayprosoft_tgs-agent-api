using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Tokens;

namespace TGS.Services.Operator.Services.GameProvider
{
    public record GetGameProvidersQuery(
        Guid BrandId,
        string CurrencyCode,
        PlayerTokenData PlayerTokenData,
        PlatformType PlatformType,
        string LanguageCode);
}