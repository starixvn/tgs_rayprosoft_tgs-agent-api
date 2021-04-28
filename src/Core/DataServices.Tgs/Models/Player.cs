using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class Player
    {
        public string IdNumber { get; set; }

        public string PlayerCode { get; set; }

        public string Username { get; set; }

        public string RealName { get; set; }

        public byte[] Password { get; set; }

        public byte[] Salt { get; set; }

        public byte[] WithdrawPassword { get; set; }

        public byte[] WithdrawSalt { get; set; }

        public Guid DefaultWalletId { get; set; }

        public string DefaultWalletCurrencyCode { get; set; }

        public Guid BrandId { get; set; }

        public virtual Brand Brand { get; set; }

        public DateTimeOffset RegisteredOn { get; set; }

        public DateTimeOffset LastActiveOn { get; set; }

        public PlayerStatus Status { get; set; }

        public int BetLimitId { get; set; }

        public virtual BetLimit BetLimit { get; set; }

        public PlayerType Type { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string QQ { get; set; }

        public string Wechat { get; set; }

        public Guid? AgentId { get; set; }
        public virtual Agent Agent { get; set; }
        public bool IdentityAuthentication { get; set; }

        public bool PhoneAuthentication { get; set; }

        public decimal AccumulatedDeposit { get; set; }

        public Guid? VipId { get; set; }

        public virtual VipLevel Vip { get; set; }

        public string UpdatedByUsername { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public DateTimeOffset? FirstDepositOn { get; set; }

        public virtual ICollection<Wallet> Wallets { get; set; }

        public virtual ICollection<PlayerBankInfo> Banks { get; set; }

        public virtual ICollection<PlayerLoginRecord> PlayerLoginRecords { get; set; }

        public virtual ICollection<StationLetterPlayer> Letters { get; set; }

        public virtual ICollection<BrandPaymentPlayer> BrandPaymentPlayers { get; set; }

        public virtual ICollection<PlayerSportsBetLimit> SportsBetLimits { get; set; }

        public virtual ICollection<PlayerBucket> PlayerBuckets { get; set; }
    }
}