using System;

namespace TGS.Services.Operator.Services.GameBalance
{
    public record FundInAllGameBalanceCommand(Guid BrandId, string PlayerCode, string GpCode);
}
