using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operations.Models.Data
{
    internal record PlayerRelatedData
    {
        internal Guid BrandId { get; init; }
        public string BrandCode { get; init; }
        public string PlayerCode { get; init; }
        public PlayerStatus Status { get; init; }
        public WalletData Wallet { get; init; }
        public string PlayerUsername { get; init; }
        public string PlayerExternalId { get; init; }
        public PlayerTypes PlayerType { get; init; }
        public string ExternalCurrencyCode { get; init; }
        public string PlayerIp { get; init; }
    }
}