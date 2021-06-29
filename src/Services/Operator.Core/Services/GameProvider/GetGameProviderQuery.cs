using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Tokens;

namespace TGS.Services.Operator.Services.GameProvider
{
    public record GetGameProviderQuery(
        Guid BrandId,
        string CurrencyCode,
        PlayerTokenData PlayerTokenData,
        PlatformType PlatformType,
        string LanguageCode);
}