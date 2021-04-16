using System.Linq;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Services.GameOperations.Models.Data;

namespace TGS.Services.GameOperations
{
    internal interface IRoundStatusManager
    {
        RoundStatus GetStatus(RoundStatusData data);

        ClientRoundStatus GetClientRoundStatus(RoundStatus status, BetValuesResult betValuesResult,
            bool gameActionWithSingleRound);
    }

    internal sealed class RoundStatusManager : IRoundStatusManager
    {
        ClientRoundStatus IRoundStatusManager.
            GetClientRoundStatus(RoundStatus status, BetValuesResult betValuesResult, bool gameActionWithSingleRound)
        {
            if (status == RoundStatus.New || status == RoundStatus.Open || betValuesResult == null)
            {
                return ClientRoundStatus.Open;
            }

            var turnover = betValuesResult.Turnover;
            var playerWinLoss = betValuesResult.WinLoss.Negate();

            if (turnover > 0)
            {
                return playerWinLoss switch
                {
                    > 0 => ClientRoundStatus.Win,
                    < 0 => ClientRoundStatus.Loss,
                    0   => ClientRoundStatus.Draw
                };
            }

            if (betValuesResult.TieBetAdjustment > 0)
            {
                return ClientRoundStatus.Draw;
            }

            // for game action stays in a single round (eg. free bet / jackpot win)
            if (gameActionWithSingleRound)
            {
                return playerWinLoss switch
                {
                    > 0 => ClientRoundStatus.Win,
                    _   => ClientRoundStatus.Loss
                };
            }

            return ClientRoundStatus.Void;
        }

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

            if (gameActionType.IsNotAllOf(GameActionType.WinBet, GameActionType.LoseBet, GameActionType.TieBet,
                GameActionType.CancelTransaction))
            {
                return oldStatus;
            }

            // AutoCloseRound=true and if all bets are settled, then close the round

            var allBetIdsInTheRound = data.ValidPreviousActions
                .Where(r => r.Reference?.Type == GameActionType.PlaceBet)
                .Select(r => r.Reference.Id).ToHashSet();

            var unsettledBetIds = data.PreviousActions
                .Where(b => b.Reference == null &&
                    b.Type.IsNotAllOf(GameActionType.FreeBet) &&
                    !allBetIdsInTheRound.Contains(b.Id))
                .Select(r => r.Id);

            // filter out current reference bet as well
            unsettledBetIds = unsettledBetIds.Except(new[] {data.ReferencedGameAction.Id});

            // can close the round if no bets left
            return !unsettledBetIds.Any() ? RoundStatus.Closed : oldStatus;
        }
    }
}