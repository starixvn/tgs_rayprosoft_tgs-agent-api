using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Bet.Models
{
    public class Round
    {
        public Guid Id { get; set; }
        public string ExternalRoundId { get; set; }
        public Guid WalletId { get; set; }
        public Guid GameId { get; set; }
        public string GameName { get; set; }
        public GameType GameType { get; set; }
        public Guid GameProviderId { get; set; }
        public string GameProviderName { get; set; }
        public string GameProviderCode { get; set; }
        public string PlayerCode { get; set; }
        public PlayerTypes PlayerType { get; set; }
        public string CurrencyCode { get; set; }
        public PlatformType PlatformType { get; set; }
        public RoundStatus Status { get; set; }
        public Guid GameProviderConfigurationId { get; set; }
        public virtual ICollection<GameAction> GameActions { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? ClosedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public Guid? LicenseeId { get; set; }
        public string LicenseeCode { get; set; }
        public string LicenseeName { get; set; }
        public DateTimeOffset? ExternalCreatedOn { get; set; }
        public DateTimeOffset? ExternalClosedOn { get; set; }
        public decimal? BetAmount { get; set; }
        public decimal? WinAmount { get; set; }
        public decimal? Commission { get; set; }
        public decimal? WinLoss { get; set; }
        public decimal? TieBetAdjustment { get; set; }

        public RoundDetail RoundDetail { get; set; }
    }
}