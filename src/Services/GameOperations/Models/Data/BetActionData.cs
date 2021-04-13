using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;

namespace TGS.Services.GameOperations.Models.Data
{
    internal class BetActionData
    {
        public Guid Id { get; init; }
        public Guid? ReferenceId { get; init; }
        public string ExternalId { get; init; }
        public string ExternalRoundId { get; init; }
        public string ExternalGameActionGroupId { get; init; }
        public BetActionData Reference { get; set; }
        public decimal Amount { get; init; }
        public decimal UnsettledBets { get; set; }
        public decimal Turnover { get; set; }
        public decimal WinLoss { get; set; }
        public bool IsClosingRound { get; init; }
        public GameActionType Type { get; init; }
        public DateTimeOffset RecordedOn { get; init; }
        public DateTimeOffset SentOn { get; init; }
        public string GameName { get; init; }
        public string GameProviderCode { get; init; }
        public string BetType { get; init; }
        public Guid WalletId { get; init; }
        public bool IsCancelled { get; set; }

        // those are used only for the current action
        // no record is kept if the previous we explicitly closed or not
        // (except in ApiCallLogs)
        public bool IsExplicitlyClosingRound { get; init; }
        public bool IsExplicitlyNotClosingRound { get; init; }
        public bool AllowCancelCancelTransaction { get; init; }

        public JackpotContribution JackpotContribution { get; init; }
    }
}