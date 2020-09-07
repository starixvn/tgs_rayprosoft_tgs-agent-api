using System;
using TGS.Core.DataServices;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;
using TGS.Infrastructures.Extensions;
using TGS.Services.Operations.Models.Data;

namespace TGS.Services.Operations.Models
{
    internal class ExecuteGameActionTransactionState
    {
        public ExecuteGameActionTransactionState(TransactionData td)
        {
            if (td == null)
                throw new ArgumentException("TransactionData passed to ExecuteGameActionTransactionState must not be NULL");

            OldRoundStatus = NewRoundStatus = RoundStatus.New;

            GameActionType = td.GameActionType;
            Timestamp = td.TimeStamp;
            Description = td.Description;
            ExternalBetId = td.ExternalBetId;
            ExternalRoundId = td.ExternalRoundId;
            ExternalTransactionId = td.ExternalTransactionId;
            ExternalTransactionReferenceId = td.ExternalTransactionReferenceId;
            ExternalGameActionGroupId = td.ExternalGameActionGroupId;
            ExternalGameId = td.ExternalGameId;
            Amount = td.Amount;
            Commission = td.Commission;
            RoundDetail = td.RoundDetail;
            RoundClosed = td.RoundClosed;
            BetType = td.BetType;
        }

        public ITgsRepository Repo { get; init; }
        public GameActionType GameActionType { get; }
        public PlatformType PlatformType { get; init; }
        public Action<decimal> ValidateAmount { get; init; }
        public string BrandCode { get; init; }
        public Func<decimal, decimal> PrepareAmount { get; init; }
        public bool HasWalletUpdate { get; init; }
        public bool MustHaveExistingRound { get; init; }
        public bool MustHaveReferencedTransaction { get; init; }
        public Guid GameProviderId { get; init; }
        public string GameProviderCode { get; init; }
        public Guid GameProviderConfigurationId { get; init; }
        public bool AutoCloseRound { get; init; }

        public PlayerRelatedData PlayerData { get; set; }
        public CachedGame[] GameProviderGames { get; set; }
        public string BrandTimeZoneId { get; set; }

        public Guid? BatchId { get; init; }
        //public bool AllowSettlementWithDifferentGameProviderConfiguration { get; set; }

        public DateTimeOffset BrandNow { get; set; }

        //public List<BetActionData> PreviousActions { get; set; }
        //public BetActionData RefGameAction { get; set; }
        //public BetValuesResult BetValueResult { get; set; }
        public Guid GameId { get; set; }

        public Guid? BetId { get; set; }
        public Guid? InternalRoundId { get; set; }
        public Guid? RefGameActionInternalId { get; set; }
        public RoundStatus OldRoundStatus { get; set; }
        public RoundStatus NewRoundStatus { get; set; }

        //public DateTimeOffset RoundCreatedOn =>
        //    PreviousActions?.FirstOrDefault()?.RecordedOn ?? BrandNow;

        //public DateTimeOffset ExternalRoundCreatedOn =>
        //    PreviousActions?.FirstOrDefault()?.SentOn ?? BrandNow;

        public DateTimeOffset? RoundClosedOn => NewRoundStatus == RoundStatus.Closed ?
            BrandNow :
            (DateTimeOffset?)null;

        // set from TransactionData
        public DateTimeOffset Timestamp { get; }

        public string Description { get; }
        public string ExternalBetId { get; }
        public string ExternalRoundId { get; set; }
        public string ExternalTransactionId { get; }
        public string ExternalTransactionReferenceId { get; set; }
        public string ExternalGameId { get; }
        public string ExternalGameActionGroupId { get; }
        public decimal Amount { get; set; }
        public decimal? Commission { get; }
        public string RoundDetail { get; }
        public bool? RoundClosed { get; }
        public string BetType { get; }

        public bool HasExtRoundId => ExternalRoundId.IsNotNullOrEmpty();
        public bool HasExtRefTransactionId => ExternalTransactionReferenceId.IsNotNullOrEmpty();

        public bool ForceGetPreviousActions { get; init; }
        public bool FreeBetAsSingleRound { get; init; }

        //public bool SkipExistingGameActionCheck { get; set; }
        //public string[] ForceCalculateTurnoverOnTieBetTypes { get; }
        //internal OppositeBetPatterns OppositeBetPatterns { get; set; }
        //internal bool AllowDuplicateTransactionIdAtDifferentRound { get; set; }
        //internal bool FreeBetAsSingleRound { get; set; }
        //internal List<BusEvent> Events { get; set; }
    }
}