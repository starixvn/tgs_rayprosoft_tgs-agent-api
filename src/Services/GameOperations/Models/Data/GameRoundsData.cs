using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.Services.GameOperations.Models.Data
{
    public record GameRoundsData
    {
        public string ExternalRoundId { get; init; }
        public Guid GameProviderId { get; init; }
        public string PlayerCode { get; init; }
        public int GameId { get; init; }
        public string CurrencyCode { get; init; }
        public RoundStatus Status { get; init; }
        public Guid GameProviderConfigurationId { get; init; }
        public Guid WalletId { get; init; }
        public DateTimeOffset CreatedOn { get; init; }
        public DateTimeOffset? ClosedOn { get; init; }
        public DateTimeOffset UpdatedOn { get; init; }
        public DateTimeOffset? ExternalCreatedOn { get; init; }
        public DateTimeOffset? ExternalClosedOn { get; init; }
        public Guid BrandId { get; init; }
        public decimal? BetAmount { get; init; }
        public decimal? ValidBetAmount { get; init; }
        public decimal? SettleAmount { get; init; }
        public decimal? Commission { get; init; }
        public decimal? Turnover { get; init; }
        public decimal? WinLoss { get; init; }
        public decimal? TieBetAdjustment { get; init; }
        public string AdditionalData { get; init; }
        public ClientRoundStatus ClientStatus { get; init; }
        public bool Success { get; init; }
    }
}
