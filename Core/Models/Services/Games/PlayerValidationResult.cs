using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Games
{
    public record PlayerValidationResult
    {
        decimal Balance { get; init; }
        string VipLevelId { get; init; }
        string GameProviderCurrency { get; init; }
        string InternalCurrency { get; init; }
        string Language { get; init; }
        string Username { get; init; }
        bool IsTestPlayer { get; init; }
        Guid BrandId { get; init; }
        string BrandCode { get; init; }
        string ResellerCode { get; init; }
        PlatformType PlatformType { get; init; }
    }
}