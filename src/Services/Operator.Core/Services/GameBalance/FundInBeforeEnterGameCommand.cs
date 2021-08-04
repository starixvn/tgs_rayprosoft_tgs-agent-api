using System;

namespace TGS.Services.Operator.Services.GameBalance
{
    public record FundInBeforeEnterGameCommand(Guid BrandId, string PlayerCode, string GpCode);
}
