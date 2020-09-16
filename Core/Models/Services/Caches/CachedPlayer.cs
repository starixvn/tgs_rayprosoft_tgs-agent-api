using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedPlayer
    {
        public string PlayerCode { get; init; }
        public Guid LicenseeId { get; init; }
        public string LicenseeCode { get; init; }
        public string LicenseeName { get; set; }
        public PlayerStatus Status { get; init; }
        public string Username { get; init; }
        public string ExternalId { get; init; }
        public string ExternalCurrencyCode { get; init; }
        public PlayerTypes Type { get; init; }
        public bool IsTestPlayer => Type.IsTestPlayer();
        public Guid DefaultWalletId { get; init; }
        public string DefaultWalletCurrencyCode { get; init; }
        public string CultureCode { get; init; }
        public int BetLimitGroupId { get; init; }
    }
}
