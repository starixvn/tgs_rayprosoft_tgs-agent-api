using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.GameOperations.Models.Data
{
    internal record SameRoundActionData
    {
        internal Guid GameActionId { get; init; }
        internal Guid? ReferenceGameActionId { get; init; }
        internal string ExternalTransactionId { get; init; }
        internal decimal Turnover { get; init; }
        internal decimal WinLoss { get; init; }
        internal decimal UnsettledBets { get; init; }
        internal bool IsClosingRound { get; init; }
        internal bool? ClosingRoundExplicitRequest { get; init; }
        internal GameActionType Type { get; init; }
        internal Guid RoundId { get; init; }
        internal Guid GameId { get; init; }
        internal RoundStatus RoundStatus { get; init; }
        internal string ExternalRoundId { get; init; }
        internal decimal Amount { get; init; }
        internal DateTimeOffset GameActionRecordedOn { get; init; }
        internal DateTimeOffset GameActionSentOn { get; init; }
        internal string BetType { get; init; }
        internal Guid WalletId { get; init; }
    }
}