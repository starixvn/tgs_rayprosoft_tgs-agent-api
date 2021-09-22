using System;

namespace TGS.Services.Operator.Services.Deposit
{
    public record SubmitDepositFormThirdPartyOrderNoCommand(Guid FormId, string ThirdPartyOrderNo);
}
