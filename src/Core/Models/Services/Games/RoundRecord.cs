using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Games
{
    public record RoundRecord
    {
        public Guid Id { get; init; }

        public string ExternalRoundId { get; init; }

        public PlatformType PlatformType { get; init; }

        public decimal? BetAmount { get; init; }

        public decimal? WinLoss { get; init; }

        public decimal? Commission { get; init; }

        public decimal? TieBetAdjustment { get; init; }

        public RoundStatus Status { get; init; }

        public DateTimeOffset? ClosedOn { get; init; }
    }
}