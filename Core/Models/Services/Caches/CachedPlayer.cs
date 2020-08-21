using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;

namespace TGS.Core.Models.Services.Caches
{
    public class CachedPlayer
    {
        public string PlayerCode { get; set; }
        public Guid BrandId { get; set; }
        public string BrandCode { get; set; }
        public Guid? BrandProxyWalletConfigurationId { get; set; }
        public bool HasProxyWallet => BrandProxyWalletConfigurationId.HasValue;
        public PlayerStatus Status { get; set; }
        public string Username { get; set; }
        public string ExternalId { get; set; }
        public string ExternalCurrencyCode { get; set; }
        public PlayerTypes Type { get; set; }
        public bool IsTestPlayer => Type.IsTestPlayer();
        public Guid DefaultWalletId { get; set; }
        public string DefaultWalletCurrencyCode { get; set; }
        public string CultureCode { get; set; }
        public int BetLimitGroupId { get; set; }

        public string LastLoginUrl { get; set; }
        public string LastCashierUrl { get; set; }
        public string LastHelpUrl { get; set; }
        public string LastTermsUrl { get; set; }
    }
}
