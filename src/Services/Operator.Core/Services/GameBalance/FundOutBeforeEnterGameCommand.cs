using System;

namespace TGS.Services.Operator.Services.GameBalance
{
    public record FundOutBeforeEnterGameCommand(Guid BrandId, string PlayerCode, string GpCode);
}
