using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class Player
    {
        public string PlayerCode { get; set; }
        public string Username { get; set; }
        public string RealName { get; set; }
        public string InvitationCode { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }

        public string CultureCode { get; set; }
        public Language Language { get; set; }
        public Guid DefaultWalletId { get; set; }
        public string DefaultWalletCurrencyCode { get; set; }
        public Guid LicenseeId { get; set; }
        public Licensee Licensee { get; set; }
        public DateTimeOffset RegisteredOn { get; set; }
        public DateTimeOffset LastActiveOn { get; set; }
        public PlayerStatus Status { get; set; }
        public int BetLimitGroupId { get; set; }
        public BetLimitGroup BetLimitGroup { get; set; }
        public PlayerTypes Type { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string QQ { get; set; }
        public string Wechat { get; set; }
        public bool HasAgent { get; set; }

        public string ParentPlayerCode { get; set; }
        public string ParentInvitationCode { get; set; }
        public Player? ParentPlayer { get; set; }

        public Guid? VipId { get; set; }
        public VipLevel Vip { get; set; }

        public string UpdatedByUsername { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }

        public virtual ICollection<Wallet> Wallets { get; set; }
        public virtual ICollection<PlayerBank> Banks { get; set; }
        public virtual ICollection<Player> SubPlayers { get; set; }
        public virtual ICollection<PlayerLoginRecord> PlayerLoginRecords { get; set; }
        public virtual ICollection<StationLetterPlayer> Letters { get; set; }
    }
}