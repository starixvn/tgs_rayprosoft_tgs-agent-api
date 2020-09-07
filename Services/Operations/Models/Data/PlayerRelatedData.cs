using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operations.Models.Data
{
    internal record PlayerRelatedData
    {
        public Guid BrandId;        
        public string BrandCode;
        public string PlayerCode;
        public PlayerStatus Status;
        public WalletData Wallet;
        public string PlayerUsername;
        public string PlayerExternalId;
        public PlayerTypes PlayerType;
        public string ExternalCurrencyCode;
        public string PlayerIp;
    }
}