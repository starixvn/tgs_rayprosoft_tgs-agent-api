using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operations.Models.Data
{
    internal record PlayerRelatedData
    {
        public Guid LicenseeId { get; init; }
        public string LicenseeCode { get; init; }
        public string LicenseeName { get; init; }
        public string PlayerCode { get; init; }
        public PlayerStatus Status { get; init; }
        public WalletData Wallet { get; init; }
        public string PlayerUsername { get; init; }
        public string PlayerExternalId { get; init; }
        public PlayerTypes PlayerType { get; init; }
        public string ExternalCurrencyCode { get; init; }        
        public string LicenseeTimeZoneId { get; init; }
        public string PlayerIp { get; init; } = string.Empty;        
    }
}