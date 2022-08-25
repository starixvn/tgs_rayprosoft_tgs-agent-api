using System.Collections.Generic;

namespace TGS.Services.Operator;

public record DepositAccumulateRouletteSetting(
    IEnumerable<DepositAccumulateRouletteItem> RouletteItems);
