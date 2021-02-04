using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.GameOperations.Models.Data
{
    internal record PlayerRelatedData
    {
        public Guid ResellerId { get; init; }
        public string ResellerCode { get; init; }
        public Guid LicenseeId { get; init; }
        public string LicenseeCode { get; init; }
        public Guid BrandId { get; init; }
        public string BrandCode { get; init; }
        public string BrandName { get; init; }
        public string PlayerCode { get; init; }
        public PlayerStatus Status { get; init; }
        public WalletData Wallet { get; init; }
        public string PlayerUsername { get; init; }
        public PlayerType PlayerType { get; init; }
        public string ExternalCurrencyCode { get; init; }
        public string BrandTimeZoneId { get; init; }
        public string PlayerIp { get; init; } = string.Empty;
    }
}