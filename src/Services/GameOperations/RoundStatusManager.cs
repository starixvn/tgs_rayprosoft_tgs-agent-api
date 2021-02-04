using System.Linq;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Services.GameOperations.Models.Data;

namespace TGS.Services.GameOperations
{
    internal interface IRoundStatusManager
    {
        RoundStatus GetStatus(RoundStatusData data);
    }

    internal sealed class RoundStatusManager : IRoundStatusManager
    {
        RoundStatus IRoundStatusManager.GetStatus(RoundStatusData data)
        {
            var requestToCloseRound = data.RequestToCloseRound;
            var gameActionType = data.GameActionType;
            var oldStatus = data.OldStatus;

            // if round has already been closed, only can be changed for re-open
            if (oldStatus == RoundStatus.Closed)
            {
                // re-open round
                if (requestToCloseRound == false &&
                    gameActionType.IsNotAllOf(GameActionType.PlaceBet))
                {
                    return RoundStatus.Open;
                }

                return RoundStatus.Closed;
            }

            // Overall:
            // 1. IF autoCloseRound=false, round must be closed explicitly.
            // 2. IF autoCloseRound=true, round is closed on any bet - ending action;
            //    if RoundClosed is passed, the round sta is determined by RoundClosed.

            if (data.AutoCloseRound == false && requestToCloseRound == null)
            {
                requestToCloseRound = false;
            }

            if (requestToCloseRound.HasValue)
            {
                return requestToCloseRound.Value ? RoundStatus.Closed : oldStatus;
            }

            // AutoCloseRound=true and if all bets are settled, then close the round

            if (gameActionType.IsOneOf(GameActionType.WinBet, GameActionType.LoseBet, GameActionType.TieBet, GameActionType.CancelTransaction))
            {
                var allBetIdsInTheRound = data.ValidPreviousActions
                    .Where(r => r.Reference?.Type == GameActionType.PlaceBet)
                    .Select(r => r.Reference.Id).ToHashSet();

                var unsettledBetIds = data.PreviousActions
                    .Where(b => b.Reference == null &&
                                b.Type.IsNotAllOf(GameActionType.FreeBet) &&
                                !allBetIdsInTheRound.Contains(b.Id))
                    .Select(r => r.Id);

                // filter out current reference bet as well
                unsettledBetIds = unsettledBetIds.Except(new[] { data.ReferencedGameAction.Id });

                // can close the round if no bets left
                if (!unsettledBetIds.Any())
                {
                    return RoundStatus.Closed;
                }
            }

            return oldStatus;
        }
    }
}