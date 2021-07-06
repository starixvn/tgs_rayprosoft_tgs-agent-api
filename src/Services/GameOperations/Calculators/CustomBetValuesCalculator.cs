using System.Collections.Generic;
using System.Linq;
using TGS.Core.Models.Extensions;
using TGS.Services.GameOperations.Models.Data;

namespace TGS.Services.GameOperations.Calculators
{
    internal class CustomBetValuesCalculator : BaseBetValuesCalculator
    {
        protected override (decimal BetAmount, decimal ValidBetAmount, decimal SettleAmount, decimal TieBetAdjustment,
            decimal Turnover, decimal WinLoss, decimal UnsettleBets) CalculateClosingActionBetValues(
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