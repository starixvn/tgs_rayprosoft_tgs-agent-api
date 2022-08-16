using System;

namespace TGS.Services.Operator.ApplicationServices.Promotions;

public record DepositAccumulateRouletteCommand(Guid PromotionId, string PlayerCode);
