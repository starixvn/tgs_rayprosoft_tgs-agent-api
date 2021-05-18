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
            IList<BetActionData> previousActions, IList<OppositeBetPattern> oppositeBetPatterns = null);
    }

    internal sealed class BetValuesCalculator : IBetValuesCalculator
    {
        BetValuesResult IBetValuesCalculator.Calculate(bool hasTurnoverOnTieBet, BetActionData current,
            IList<BetActionData> previousActions, IList<OppositeBetPattern> oppositeBetPatterns)
        {
            if (current.Type.IsOneOf(LockAmount, UnlockAmount))
            {
                return new();
            }

            decimal betAmount = 0, validBetAmount = 0, settleAmount = 0;
            decimal turnover = 0, winLoss = 0;
            decimal unsettledBets = 0, tieBetAdjustment = 0;

            previousActions ??= Array.Empty<BetActionData>();
            oppositeBetPatterns ??= Array.Empty<OppositeBetPattern>();
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
                    CalculateClosingActionBetValues(hasTurnoverOnTieBet, current, previousActions, oppositeBetPatterns);
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
                bool hasTurnoverOnTieBet, BetActionData current,
                IList<BetActionData> previousActions, IList<OppositeBetPattern> oppositeBetPatterns)
        {
            var allActions = GetAllActions(current, previousActions);

            var betAmount = CalculateBetAmount(allActions);
            var settleAmount = CalculateSettleAmount(allActions);
            var winLoss = CalculateWinLoss(allActions);
            var unsettledBets = CalculateUnsettledBets(previousActions);

            var (turnover, validBetAmount, tieBetAdjustment) =
                CalculateTurnoverWithValidBetAmountAndTieBetAdjustment(hasTurnoverOnTieBet, allActions,
                    oppositeBetPatterns);

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
                .Where(a => (a.Type.IsSettleBet() || a.Type == WinJackpot) && a.IsCancelled == false)
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


        private static (decimal Turnover, decimal ValidBetAmount, decimal tieBetAdjustment)
            CalculateTurnoverWithValidBetAmountAndTieBetAdjustment(bool hasTurnoverOnTieBet,
                IList<BetActionData> allActions, IList<OppositeBetPattern> oppositeBetPatterns)
        {
            decimal turnover = 0, tieBetAdjustment = 0;
            var validBetAmount = CalculateValidBetAmount(allActions);
            var jackpotContributionAmount = allActions.Where(a => a.JackpotContribution != null)
                .Sum(a => a.JackpotContribution.TotalAmount);


            var allBets = allActions.Where(a => a.Type == PlaceBet && a.IsCancelled == false).ToArray();
            var matchedPatternGroupIds = (from a in allBets
                                          from p in oppositeBetPatterns
                                          where a.BetType == p.BetType
                                          select p.GroupId).Distinct().ToArray();

            if (matchedPatternGroupIds.Length == 0)
            {
                tieBetAdjustment = CalculateTieBetAdjustment(hasTurnoverOnTieBet, allActions);

                turnover = validBetAmount - tieBetAdjustment;
                turnover = CalculateTurnoverWithJackpotContribution(turnover, jackpotContributionAmount);

                return (turnover, validBetAmount, tieBetAdjustment);
            }

            // calculate turnover if opposite bets exist
            decimal rollingTurnover = 0;

            var uncalculatedBets = allBets;
            foreach (var id in matchedPatternGroupIds)
            {
                var betAmountOfBetTypes = oppositeBetPatterns.Where(p => p.GroupId == id)
                    .Select(p => p.BetType).ToDictionary(p => p, _ => 0m);

                var oppositeBetTypes = betAmountOfBetTypes.Keys;
                var oppositeBets = allBets.Where(a => oppositeBetTypes.Contains(a.BetType)).ToArray();

                foreach (var bet in oppositeBets)
                {
                    var betAmount = bet.Amount.Negate();
                    var tieBet = allActions.FirstOrDefault(a => a.ReferenceId == bet.Id && a.Type == TieBet);
                    var adjustment = hasTurnoverOnTieBet || tieBet == null ? 0 : tieBet.Amount;
                    var actualBetAmount = betAmount - adjustment;

                    betAmountOfBetTypes[bet.BetType] += actualBetAmount;
                    tieBetAdjustment += adjustment;
                }

                rollingTurnover += CalculateRollingTurnover(betAmountOfBetTypes);

                uncalculatedBets = uncalculatedBets.Except(oppositeBets).ToArray();
            }

            turnover = uncalculatedBets.Sum(a => a.Amount).Negate() + rollingTurnover;
            turnover = CalculateTurnoverWithJackpotContribution(turnover, jackpotContributionAmount);

            return (turnover, validBetAmount, tieBetAdjustment);
        }

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

        private static decimal CalculateRollingTurnover(Dictionary<string, decimal> betAmountOfBetTypes)
        {
            // substract the bet amount of opposite bets, and return absolute value
            var rolling = betAmountOfBetTypes.Values.Select((v, i) => i == 0 ? v : v.Negate()).Sum();
            return Math.Abs(rolling);
        }

        private static decimal CalculateTurnoverWithJackpotContribution(decimal turnover,
            decimal jackpotContributionAmount)
        {
            var contrib = turnover > 0 ? jackpotContributionAmount : 0;
            return turnover - contrib;
        }
    }
}