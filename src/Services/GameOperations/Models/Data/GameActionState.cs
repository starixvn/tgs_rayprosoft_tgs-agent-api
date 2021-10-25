using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;

namespace TGS.Services.GameOperations.Models.Data
{
    internal class GameActionState
    {
        internal GameActionState(TransactionData td)
        {
            if (td == null)
                throw new ArgumentException(
                    "TransactionData passed to ExecuteGameActionTransactionState must not be NULL");

            OldRoundStatus = NewRoundStatus = RoundStatus.New;

            GameActionType = td.GameActionType;
            Timestamp = td.TimeStamp;
            ExtraData = td.ExtraData ?? string.Empty;
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
            JackpotContribution = td.JackpotContribution;
            CustomBetValues = td.CustomBetValues;

            UtcNow = DateTimeOffset.UtcNow;
        }

        public bool AllowCancelCancelTransaction { get; init; }
        public decimal Amount { get; set; }
        public bool AutoCloseRound { get; init; }

        public Guid? BatchId { get; init; }
        public string BetType { get; init; }
        public BetValuesResult BetValueResult { get; set; }
        public decimal? Commission { get; init; }
        public CustomBetValues CustomBetValues { get; }
        public string ExternalBetId { get; init; }
        public string ExternalGameActionGroupId { get; init; }
        public string ExternalGameId { get; init; }

        public DateTimeOffset ExternalRoundCreatedOn =>
            PreviousActions?.FirstOrDefault()?.SentOn ?? UtcNow;

        public string ExternalRoundId { get; init; }
        public string ExternalTransactionId { get; init; }
        public string ExternalTransactionReferenceId { get; init; }

        public string ExtraData { get; init; }

        public bool ForceGetPreviousActions { get; init; }
        public bool FreeBetAsSingleRound { get; init; }
        public CachedGame Game { get; set; }
        public GameActionType GameActionType { get; }
        public int GameId { get; set; }
        public CachedGameProvider GameProvider { get; init; }
        public Guid GameProviderConfigurationId { get; init; }
        public CachedGame[] GameProviderGames { get; init; }
        public bool HasExtBetId => ExternalBetId.IsNotNullOrEmpty();
        public bool HasExternalGameActionGroupId => ExternalGameActionGroupId.IsNotNullOrEmpty();
        public bool HasExtGameId => ExternalGameId.IsNotNullOrEmpty();
        public bool HasExtRefTransactionId => ExternalTransactionReferenceId.IsNotNullOrEmpty();

        public bool HasExtRoundId => ExternalRoundId.IsNotNullOrEmpty();
        public bool HasWalletUpdate { get; init; }
        public Guid? InternalRoundId { get; set; }
        public JackpotContribution JackpotContribution { get; }
        public bool MustHaveExistingRound { get; init; }
        public bool MustHaveReferencedTransaction { get; init; }
        public RoundStatus NewRoundStatus { get; set; }
        public RoundStatus OldRoundStatus { get; set; }

        public OppositeBetRule[] OppositeBetRules { get; init; }
        public PlatformType? PlatformType { get; init; }

        public PlayerRelatedData PlayerData { get; init; }
        public Func<decimal, decimal> PrepareAmount { get; init; }

        /// <summary>
        /// all game actions in the same round
        /// </summary>
        public IList<BetActionData> PreviousActions { get; set; } = Array.Empty<BetActionData>();

        public BetActionData ReferencedGameAction { get; set; }

        public Guid? ReferencedGameActionInternalId { get; set; }

        public ITgsRepository Repository { get; init; }
        public bool? RoundClosed { get; init; }

        public DateTimeOffset? RoundClosedOn =>
            NewRoundStatus == RoundStatus.Closed ? UtcNow : default(DateTimeOffset?);

        public DateTimeOffset RoundCreatedOn =>
            PreviousActions?.FirstOrDefault()?.RecordedOn ?? UtcNow;

        public string RoundDetail { get; init; }

        // set from TransactionData
        public DateTimeOffset Timestamp { get; init; }

        public DateTimeOffset UtcNow { get; }
        public Action<string, decimal> ValidateAmount { get; init; }

        /// <summary>
        /// game actions except that is cancelled in the same round
        /// </summary>
        public IList<BetActionData> ValidPreviousActions { get; set; } = Array.Empty<BetActionData>();

        public bool WithCustomBetValues { get; init; }
    }
}