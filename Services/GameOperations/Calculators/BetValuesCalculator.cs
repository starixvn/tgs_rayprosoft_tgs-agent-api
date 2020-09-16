using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Extensions;
using TGS.Services.GameOperations.Models.Data;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Services.GameOperations.Calculators
{
    internal interface IBetValuesCalculator
    {
        BetValuesResult Calculate(bool hasTurnoverOnTieBet, BetActionData current, IList<BetActionData> previousActions);
    }

    internal sealed class BetValuesCalculator : IBetValuesCalculator
    {
        BetValuesResult IBetValuesCalculator.Calculate(bool hasTurnoverOnTieBet, BetActionData current, IList<BetActionData> previousActions)
        {
            if (current.Type.IsOneOf(LockAmount, UnlockAmount))
            {
                return new();
            }

            var betAmount = default(decimal);
            var turnover = default(decimal);
            var winloss = default(decimal);
            var unsettledBets = default(decimal);
            var tiebetAdjustment = default(decimal);

            previousActions ??= Array.Empty<BetActionData>();
            var previouslyClosed = FindPreviouslyClosed(previousActions);

            // round has already been closed
            // and re-opening round
            if (previouslyClosed != null && current.IsExplicitlyNotClosingRound)
            {
                var (Turnover, WinLoss, UnsettleBets) = ReversePreviouslyClosedBetValues(previouslyClosed);
                turnover = Turnover;
                winloss = WinLoss;
                unsettledBets = UnsettleBets;
            }
            // round is open and we are closing it now
            else if (current.IsClosingRound)
            {
                var (BetAmount, TieBetAdjustment, Turnover, WinLoss, UnsettleBets) = CalculateClosingActionBetValues(hasTurnoverOnTieBet, current, previousActions);

                betAmount = BetAmount;
                turnover = Turnover;
                winloss = WinLoss;
                unsettledBets = UnsettleBets;
                tiebetAdjustment = TieBetAdjustment;
            }
            // round is open and we are placing bet and not closing it
            else if (current.Type == PlaceBet)
            {
                unsettledBets = current.Amount.Negate();
            }

            return new BetValuesResult()
            {
                BetAmount = betAmount,
                Turnover = turnover,
                WinLoss = winloss,
                UnsettledBets = unsettledBets,
                TieBetAdjustment = tiebetAdjustment
            };
        }

        private BetActionData FindPreviouslyClosed(IList<BetActionData> actions)
        {
            if (actions.Count == 0)
                return null;

            BetActionData closingAction = null;
            foreach (var a in actions)
            {
                if (a.IsClosingRound)
                {
                    closingAction = a;
                }
                else if (closingAction != null && a.IsExplicitlyNotClosingRound)
                {
                    closingAction = null;
                }
            }

            return closingAction;
        }

        private (decimal Turnover, decimal WinLoss, decimal UnsettleBets) ReversePreviouslyClosedBetValues(BetActionData previouslyClosed)
        {
            var turnover = previouslyClosed.Turnover.Negate();
            var winloss = previouslyClosed.WinLoss.Negate();
            var unsettleBets = previouslyClosed.UnsettledBets.Negate();

            return (turnover, winloss, unsettleBets);
        }

        private (decimal BetAmount, decimal TiebetAdjustment, decimal Turnover, decimal WinLoss, decimal UnsettleBets)
            CalculateClosingActionBetValues(bool hasTurnoverOnTieBet, BetActionData current, IList<BetActionData> previousActions)
        {
            var allActions = previousActions.Concat(new[] { current }).ToArray();

            var betAmount = CalculateBetAmount(allActions);
            var tiebetAdjustment = CalculateTiebetAdjustment(hasTurnoverOnTieBet, allActions);
            var winloss = CalculateWinLoss(allActions);
            var unsettledBets = CalculateUnsettledBets(previousActions);

            var turnover = betAmount - tiebetAdjustment;

            return (betAmount, tiebetAdjustment, turnover, winloss, unsettledBets);
        }

        private static decimal CalculateBetAmount(IList<BetActionData> allActions)
        {
            // sum all amounts of PlaceBet without cancelled, place bet value is stored as negative
            return allActions
                .Where(a => a.Type == PlaceBet && a.IsCancelled == false)
                .Sum(a => a.Amount)
                .Negate();
        }

        private static decimal CalculateTiebetAdjustment(bool hasTurnoverOnTieBet, IList<BetActionData> allActions)
        {
            // if has turnover on tie bet do nothing
            // if has NO turnover on tie bet, then exclude those expect the bet types that need to calculate turnover forcibly

            return hasTurnoverOnTieBet
                ? 0m
                : allActions
                    .Where(a => a.Type == TieBet &&
                                a.Reference != null &&
                                a.Reference.Type == PlaceBet &&
                                a.IsCancelled == false)
                    .Sum(a => a.Reference.Amount)
                    .Negate();
        }

        private static decimal CalculateWinLoss(IList<BetActionData> allActions)
        {
            // here it means company WinLoss
            // WinLoss = -1 * Sum(Amount from all)
            return allActions.Sum(a => a.Amount).Negate();
        }

        private static decimal CalculateUnsettledBets(IList<BetActionData> previousActions)
        {
            // UnsettledBets = -1 * Sum(UnsettledBets)
            return previousActions.Sum(a => a.UnsettledBets).Negate();
        }
    }
}