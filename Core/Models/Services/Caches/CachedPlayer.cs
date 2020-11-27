using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedPlayer
    {
        public string PlayerCode { get; init; }
        public Guid BrandId { get; init; }
        public string BrandCode { get; init; }
        public PlayerStatus Status { get; init; }
        public string Username { get; init; }
        public string ExternalCurrencyCode { get; init; }
        public PlayerType Type { get; init; }
        public bool IsTestPlayer => Type.IsTestPlayer();
        public Guid DefaultWalletId { get; init; }
        public string DefaultWalletCurrencyCode { get; init; }
        public string CultureCode { get; init; }
        public int BetLimitGroupId { get; init; }
    }
}