using System;
using System.IO;

namespace TGS.Services.Operator.Deposit
{
    public record SubmitBankDepositFormCommand(
        string PlayerCode,
        Guid FormId,
        string ContentType,
        Stream FileStream);
}
