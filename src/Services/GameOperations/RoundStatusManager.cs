using System.Linq;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Services.GameOperations.Models.Data;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Services.GameOperations
{
    internal interface IRoundStatusManager
    {
        RoundStatus GetStatus(RoundStatusData data);

        ClientRoundStatus GetClientRoundStatus(RoundStatus status, BetValuesResult betValuesResult,
            bool gameActionWithSingleRound, bool isVoidRound);
    }

    internal sealed class RoundStatusManager : IRoundStatusManager
    {
        ClientRoundStatus IRoundStatusManager.
            GetClientRoundStatus(RoundStatus status, BetValuesResult betValuesResult, bool gameActionWithSingleRound,
                bool isVoidRound)
        {
            if (status is RoundStatus.New or RoundStatus.Open || betValuesResult == null)
            {
                return ClientRoundStatus.Open;
            }

            if (isVoidRound)
            {
                return ClientRoundStatus.Void;
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

            // for opposite bet or promotion, it will has zero turnover and positive settle amount
            if (betValuesResult.SettleAmount > 0)
            {
                return playerWinLoss switch
                {
                    > 0 => ClientRoundStatus.Win,
                    _   => ClientRoundStatus.Loss
                };
            }

            return ClientRoundStatus.Loss;
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
                    gameActionType.IsNotAllOf(PlaceBet, WinJackpot))
                {
                    return RoundStatus.Open;
                }

                return RoundStatus.Closed;
            }

            // Overall:
            // 1. If autoCloseRound=false, round must be closed explicitly.
            // 2. If autoCloseRound=true, round is closed on any bet - ending action;
            //    if RoundClosed is passed, the round sta is determined by RoundClosed.

            if (data.AutoCloseRound == false && requestToCloseRound == null)
            {
                requestToCloseRound = false;
            }

            if (requestToCloseRound.HasValue)
            {
                return requestToCloseRound.Value ? RoundStatus.Closed : oldStatus;
            }

            if (gameActionType.IsNotAllOf(WinBet, LoseBet, TieBet,
                CancelTransaction))
            {
                return oldStatus;
            }

            // AutoCloseRound=true and if all bets are settled, then close the round

            var allBetIdsInTheRound = data.ValidPreviousActions
                .Where(r => r.Reference?.Type == PlaceBet)
                .Select(r => r.Reference.Id).ToHashSet();

            var unsettledBetIds = data.PreviousActions
                .Where(b => b.Reference == null &&
                    b.Type.IsNotAllOf(FreeBet, LockAmount, UnlockAmount) &&
                    !allBetIdsInTheRound.Contains(b.Id))
                .Select(r => r.Id);

            // filter out current reference bet as well
            unsettledBetIds = unsettledBetIds.Except(new[] {data.ReferencedGameAction.Id});

            // can close the round if no bets left
            return !unsettledBetIds.Any() ? RoundStatus.Closed : oldStatus;
        }
    }
}