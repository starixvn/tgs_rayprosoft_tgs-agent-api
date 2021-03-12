using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Data
{
    public record GameActionResult
    {
        public Guid Id { get; init; }

        public decimal Amount { get; init; }

        public GameActionType GameActionType { get; init; }

        public string ExternalTransactionId { get; init; }

        public string ExternalBetId { get; init; }

        public string ExternalGameActionGroupId { get; init; }

        public Guid? ReferencedGameActionId { get; init; }

        public DateTimeOffset RecordedOn { get; init; }

        public Guid RoundId { get; init; }

        public string ExternalRoundId { get; init; }

        public PlatformType PlatformType { get; init; }

        public decimal? RoundBetAmount { get; init; }

        public decimal? RoundValidBetAmount { get; init; }

        public decimal? RoundWinLoss { get; init; }

        public decimal? Commission { get; init; }

        public decimal? TieBetAdjustment { get; init; }

        public RoundStatus RoundStatus { get; init; }

        public DateTimeOffset? RoundClosedOn { get; init; }
    }
}