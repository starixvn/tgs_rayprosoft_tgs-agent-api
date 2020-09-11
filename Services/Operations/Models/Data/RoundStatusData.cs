using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operations.Models.Data
{
    public record RoundStatusData
    {
        public bool AutoCloseRound { get; init; }
        public bool? RequestToCloseRound { get; init; }
        public Guid RoundId { get; init; }
        public RoundStatus OldStatus { get; init; }
        public GameActionType GameActionType { get; init; }
        public BetActionData ReferencedGameAction { get; init; }
        public IList<BetActionData> PreviousActions { get; init; }
        public IList<BetActionData> ValidPreviousActions { get; init; }
    }
}