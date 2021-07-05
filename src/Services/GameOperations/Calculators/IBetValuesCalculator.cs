// unset

using System.Collections.Generic;
using TGS.Services.GameOperations.Models.Data;

namespace TGS.Services.GameOperations.Calculators
{
    internal interface IBetValuesCalculator
    {
        BetValuesResult Calculate(bool hasTurnoverOnTieBet, BetActionData current,
            IList<BetActionData> previousActions, IList<OppositeBetPattern> oppositeBetPatterns = null);
    }
}