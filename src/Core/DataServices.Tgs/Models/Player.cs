using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

#nullable enable

namespace TGS.Core.DataServices.Tgs.Models
{
    public class Player
    {
        public decimal AccumulatedDeposit { get; set; }

        public virtual Agent Agent { get; set; } = default!;

        public Guid? AgentId { get; set; }

        public string? AreaCode { get; set; }

        public virtual ICollection<PlayerBankInfo> Banks { get; set; } = default!;

        public virtual BetLimit BetLimit { get; set; } = default!;

        public int BetLimitId { get; set; }

        public DateTimeOffset? Birthday { get; set; }

        public virtual Brand Brand { get; set; } = default!;

        public Guid BrandId { get; set; }

        public virtual ICollection<BrandPaymentPlayer> BrandPaymentPlayers { get; set; } = default!;

        public string DefaultWalletCurrencyCode { get; set; } = default!;

        public Guid DefaultWalletId { get; set; }

        public string? Email { get; set; }

        public DateTimeOffset? FirstDepositOn { get; set; }

        public DateTimeOffset? FirstPromotionOn { get; set; }

        public virtual PlayerGameSetting GameSetting { get; set; } = default!;

        public bool IdentityAuthentication { get; set; }

        public string? IdNumber { get; set; }

        public DateTimeOffset LastActiveOn { get; set; }

        public virtual ICollection<StationLetterPlayer> Letters { get; set; } = default!;

        public byte[] Password { get; set; } = default!;

        public string? Phone { get; set; }

        public bool PhoneAuthentication { get; set; }

        public virtual ICollection<PlayerBucket> PlayerBuckets { get; set; } = default!;

        public string PlayerCode { get; set; } = default!;

        public virtual ICollection<PlayerLoginRecord> PlayerLoginRecords { get; set; } = default!;

        public string? QQ { get; set; }

        public string? RealName { get; set; }

        public DateTimeOffset? RegisteredOn { get; set; }

        public string? Remark { get; set; }

        public byte[] Salt { get; set; } = default!;

        public virtual ICollection<PlayerSportsBetLimit> SportsBetLimits { get; set; } = default!;

        public PlayerStatus Status { get; set; }

        public PlayerType Type { get; set; }

        public string? UpdatedByUsername { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string Username { get; set; } = default!;

        public virtual VipLevel Vip { get; set; } = default!;

        public Guid? VipId { get; set; }

        public virtual ICollection<Wallet> Wallets { get; set; } = default!;

        public string? Wechat { get; set; }

        public byte[] WithdrawPassword { get; set; } = default!;

        public byte[] WithdrawSalt { get; set; } = default!;
    }
}
