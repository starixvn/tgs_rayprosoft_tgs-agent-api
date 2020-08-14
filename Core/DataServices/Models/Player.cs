using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class Player
    {
        public string PlayerCode { get; set; }
        public string Username { get; set; }
        public string ExternalId { get; set; }
        public string IpAddress { get; set; }
        public string CultureCode { get; set; }
        public Language Language { get; set; }
        public Guid DefaultWalletId { get; set; }
        public string DefaultWalletCurrencyCode { get; set; }
        public Guid BrandId { get; set; }
        public Brand Brand { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastActiveOn { get; set; }
        public PlayerStatus Status { get; set; }
        public int BetLimitGroupId { get; set; }
        public PlayerTypes Type { get; set; }
        public string LastLoginUrl { get; set; }
        public string LastCashierUrl { get; set; }
        public string LastHelpUrl { get; set; }
        public string LastTermsUrl { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}