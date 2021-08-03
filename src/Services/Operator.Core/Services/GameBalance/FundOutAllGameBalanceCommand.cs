using System;

namespace TGS.Services.Operator.Services.GameBalance
{
    public record FundOutAllGameBalanceCommand(Guid BrandId, string PlayerCode, string GpCode);
}
