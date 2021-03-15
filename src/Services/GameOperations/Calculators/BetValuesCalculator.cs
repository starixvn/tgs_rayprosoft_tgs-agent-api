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
        BetValuesResult Calculate(bool hasTurnoverOnTieBet, BetActionData current,
            IList<BetActionData> previousActions);
    }

    internal sealed class BetValuesCalculator : IBetValuesCalculator
    {
        BetValuesResult IBetValuesCalculator.Calculate(bool hasTurnoverOnTieBet, BetActionData current,
            IList<BetActionData> previousActions)
        {
            if (current.Type.IsOneOf(LockAmount, UnlockAmount))
            {
                return new();
            }

            decimal betAmount = 0, validBetAmount = 0, settleAmount = 0;
            decimal turnover = 0, winLoss = 0;
            decimal unsettledBets = 0, tieBetAdjustment = 0;

            previousActions ??= Array.Empty<BetActionData>();
            var previouslyClosed = FindPreviouslyClosed(previousActions);

            // round has already been closed
            // and re-opening round
            if (previouslyClosed != null && current.IsExplicitlyNotClosingRound)
            {
                (turnover, winLoss, unsettledBets) = ReversePreviouslyClosedBetValues(previouslyClosed);
                (betAmount, validBetAmount) = CalculateBetAmounts(current, previousActions);
            }
            // round is open and we are closing it now
            else if (current.IsClosingRound)
            {
                (betAmount, validBetAmount, settleAmount, tieBetAdjustment, turnover, winLoss, unsettledBets) =
                    CalculateClosingActionBetValues(hasTurnoverOnTieBet, current, previousActions);
            }
            else
            {
                switch (current.Type)
                {
                    case PlaceBet:
                        (betAmount, validBetAmount, unsettledBets) =
                            CalculatePlaceBetActionBetValues(current, previousActions);
                        break;

                    case CancelTransaction:
                        (betAmount, validBetAmount) = CalculateBetAmounts(current, previousActions);
                        break;
                }
            }

            return new()
            {
                BetAmount = betAmount,
                ValidBetAmount = validBetAmount,
                SettleAmount = settleAmount,
                Turnover = turnover,
                WinLoss = winLoss,
                UnsettledBets = unsettledBets,
                TieBetAdjustment = tieBetAdjustment
            };
        }

        private static BetActionData FindPreviouslyClosed(IList<BetActionData> actions)
        {
            if (actions.Count == 0)
            {
                return null;
            }

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

        private static (decimal Turnover, decimal WinLoss, decimal UnsettleBets) ReversePreviouslyClosedBetValues(
            BetActionData previouslyClosed)
        {
            var turnover = previouslyClosed.Turnover.Negate();
            var winLoss = previouslyClosed.WinLoss.Negate();
            var unsettleBets = previouslyClosed.UnsettledBets.Negate();

            return (turnover, winLoss, unsettleBets);
        }

        private static (decimal BetAmount, decimal ValidBetAmount, decimal SettleAmount, decimal TieBetAdjustment,
            decimal Turnover, decimal WinLoss, decimal UnsettleBets) CalculateClosingActionBetValues(
                bool hasTurnoverOnTieBet,
                BetActionData current, IList<BetActionData> previousActions)
        {
            var allActions = GetAllActions(current, previousActions);

            var betAmount = CalculateBetAmount(allActions);
            var validBetAmount = CalculateValidBetAmount(allActions);
            var settleAmount = CalculateSettleAmount(allActions);
            var tieBetAdjustment = CalculateTieBetAdjustment(hasTurnoverOnTieBet, allActions);
            var winLoss = CalculateWinLoss(allActions);
            var unsettledBets = CalculateUnsettledBets(previousActions);

            var turnover = validBetAmount - tieBetAdjustment;

            return (betAmount, validBetAmount, settleAmount, tieBetAdjustment, turnover, winLoss, unsettledBets);
        }

        private static (decimal BetAmount, decimal validBetAmount, decimal UnsettleBets)
            CalculatePlaceBetActionBetValues(BetActionData current, IList<BetActionData> previousActions)
        {
            var allActions = GetAllActions(current, previousActions);
            var betAmount = CalculateBetAmount(allActions);
            var validBetAmount = CalculateValidBetAmount(allActions);

            var unsettledBets = current.Amount.Negate();

            return (betAmount, validBetAmount, unsettledBets);
        }

        private static (decimal betAmount, decimal validBetAmount) CalculateBetAmounts(BetActionData current,
            IList<BetActionData> previousActions)
        {
            var allActions = GetAllActions(current, previousActions);
            var betAmount = CalculateBetAmount(allActions);
            var validBetAmount = CalculateValidBetAmount(allActions);

            return (betAmount, validBetAmount);
        }

        private static decimal CalculateBetAmount(IList<BetActionData> allActions) =>
            // sum all amounts of PlaceBet, place bet value is stored as negative
            allActions
                .Where(a => a.Type == PlaceBet)
                .Sum(a => a.Amount)
                .Negate();

        private static decimal CalculateValidBetAmount(IList<BetActionData> allActions) =>
            // sum all amounts of PlaceBet that are not cancelled, place bet value is stored as negative
            allActions
                .Where(a => a.Type == PlaceBet && a.IsCancelled == false)
                .Sum(a => a.Amount)
                .Negate();

        private static decimal CalculateSettleAmount(IList<BetActionData> allActions) =>
            // sum all amounts of settle actions without cancelled, include free bets
            allActions
                .Where(a => a.Type.IsSettleBet() && a.IsCancelled == false)
                .Sum(a => a.Amount);

        private static decimal CalculateTieBetAdjustment(bool hasTurnoverOnTieBet, IList<BetActionData> allActions) =>
            // if has turnover on tie bet do nothing
            // if has NO turnover on tie bet, then exclude those expect the bet types that need to calculate turnover forcibly
            hasTurnoverOnTieBet
                ? 0m
                : allActions
                    .Where(a => a.Type == TieBet &&
                        a.Reference != null &&
                        a.Reference.Type == PlaceBet &&
                        a.IsCancelled == false)
                    .Sum(a => a.Reference.Amount)
                    .Negate();

        private static decimal CalculateWinLoss(IList<BetActionData> allActions) =>
            // here it means company WinLoss
            // WinLoss = -1 * Sum(Amount from all)
            allActions.Sum(a => a.Amount).Negate();

        private static decimal CalculateUnsettledBets(IList<BetActionData> previousActions) =>
            // UnsettledBets = -1 * Sum(UnsettledBets)
            previousActions.Sum(a => a.UnsettledBets).Negate();

        private static IList<BetActionData>
            GetAllActions(BetActionData current, IList<BetActionData> previousActions) =>
            previousActions.Concat(new[] {current}).ToArray();
    }
}