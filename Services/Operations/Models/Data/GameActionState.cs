using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Core.DataServices;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;
using TGS.Infrastructures.Extensions;
using TGS.Services.Operations.Models.Data;

namespace TGS.Services.Operations.Models
{
    internal class GameActionState
    {
        internal GameActionState(TransactionData td)
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
        public GameActionType GameActionType { get; init; }
        public PlatformType PlatformType { get; init; }
        public Action<string, decimal> ValidateAmount { get; init; }
        public string LicenseeCode { get; init; }
        public Func<decimal, decimal> PrepareAmount { get; init; }
        public bool HasWalletUpdate { get; init; }
        public bool MustHaveExistingRound { get; init; }
        public bool MustHaveReferencedTransaction { get; init; }
        public Guid GameProviderId { get; init; }
        public string GameProviderCode { get; init; }
        public Guid GameProviderConfigurationId { get; init; }
        public bool AutoCloseRound { get; init; }

        public PlayerRelatedData PlayerData { get; init; }
        public CachedGame[] GameProviderGames { get; init; }

        public Guid? BatchId { get; init; }
        //public bool AllowSettlementWithDifferentGameProviderConfiguration { get; set; }

        public DateTimeOffset LicenseeNow { get; set; }

        /// <summary>
        /// all game actions in the same round
        /// </summary>
        public IList<BetActionData> PreviousActions { get; set; }

        /// <summary>
        /// game actions except that is cancelled in the same round
        /// </summary>
        public IList<BetActionData> ValidPreviousActions { get; set; } 
        
        public Guid? ReferencedGameActionInternalId { get; set; }
        public BetActionData ReferencedGameAction { get; set; }
                //public BetValuesResult BetValueResult { get; set; }
        public Guid GameId { get; set; }
        public CachedGame Game { get; set; }

        public Guid? BetId { get; set; }
        public Guid? InternalRoundId { get; set; }
        public RoundStatus OldRoundStatus { get; set; }
        public RoundStatus NewRoundStatus { get; set; }

        public DateTimeOffset RoundCreatedOn =>
            PreviousActions?.FirstOrDefault()?.RecordedOn ?? LicenseeNow;

        public DateTimeOffset ExternalRoundCreatedOn =>
            PreviousActions?.FirstOrDefault()?.SentOn ?? LicenseeNow;

        public DateTimeOffset? RoundClosedOn => NewRoundStatus == RoundStatus.Closed ?
            LicenseeNow :
            (DateTimeOffset?)null;

        // set from TransactionData
        public DateTimeOffset Timestamp { get; init; }

        public string Description { get; init; }
        public string ExternalBetId { get; init; }
        public string ExternalRoundId { get; init; }
        public string ExternalTransactionId { get; init; }
        public string ExternalTransactionReferenceId { get; init; }
        public string ExternalGameId { get; init; }
        public string ExternalGameActionGroupId { get; init; }
        public decimal Amount { get; set; }
        public decimal? Commission { get; init; }
        public string RoundDetail { get; init; }
        public bool? RoundClosed { get; init; }
        public string BetType { get; init; }

        public bool HasExtRoundId => ExternalRoundId.IsNotNullOrEmpty();
        public bool HasExtRefTransactionId => ExternalTransactionReferenceId.IsNotNullOrEmpty();
        public bool HasExtGameId => ExternalGameId.IsNotNullOrEmpty();

        public bool ForceGetPreviousActions { get; init; }
        public bool FreeBetAsSingleRound { get; init; }

        //public string[] ForceCalculateTurnoverOnTieBetTypes { get; }
        //internal OppositeBetPatterns OppositeBetPatterns { get; set; }
        //internal bool AllowDuplicateTransactionIdAtDifferentRound { get; set; }
        //internal List<BusEvent> Events { get; set; }
    }
}