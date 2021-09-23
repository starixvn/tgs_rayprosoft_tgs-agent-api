using System.Collections.Generic;
using System.Linq;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Extensions;
using TGS.Services.GameOperations.Calculators;
using TGS.Services.GameOperations.Models.Data;

namespace TGS.Apis.TgsApi.GameProviders.Card365.Services
{
    public class Card365BetValuesCalculator : BaseBetValuesCalculator
    {
        protected override
            (decimal BetAmount,
            decimal ValidBetAmount,
            decimal Turnover,
            decimal WinLoss,
            decimal UnsettleBets)
            ReversePreviouslyClosedBetValues(
                BetActionData current,
                IList<BetActionData> previousActions,
                BetActionData previouslyClosed)
        {
            var settleActions = previousActions
                .Where(a => a.Type.IsSettleBet() && a.IsCancelled == false).ToArray();

            var turnover = settleActions.Sum(a => a.Turnover).Negate();
            var winLoss = settleActions.Sum(a => a.WinLoss).Negate();

            var unsettledBets = previouslyClosed.UnsettledBets.Negate();

            var (betAmount, validBetAmount) = CalculateBetAmounts(current, previousActions);

            return (betAmount, validBetAmount, turnover, winLoss, unsettledBets);
        }

        protected override
            (decimal BetAmount,
            decimal ValidBetAmount,
            decimal SettleAmount,
            decimal TieBetAdjustment,
            decimal Turnover,
            decimal WinLoss,
            decimal UnsettleBets)
            CalculateClosingActionBetValues(
                bool hasTurnoverOnTieBet,
                BetActionData current,
                IList<BetActionData> previousActions,
                IList<OppositeBetPattern> oppositeBetPatterns)
        {
            var allActions = GetAllActions(current, previousActions);

            var betAmount = CalculateBetAmount(allActions);
            var settleAmount = CalculateSettleAmount(allActions);
            var validBetAmount = CalculateValidBetAmount(allActions);
            var unsettledBets = CalculateUnsettledBets(previousActions);

            var settleActions = allActions
                .Where(a => a.Type.IsSettleBet() && a.IsCancelled == false).ToArray();

            var turnover = settleActions.Sum(a => a.Turnover);
            var winLoss = settleActions.Sum(a => a.WinLoss);

            return (betAmount, validBetAmount, settleAmount, 0m, turnover, winLoss, unsettledBets);
        }
    }
}