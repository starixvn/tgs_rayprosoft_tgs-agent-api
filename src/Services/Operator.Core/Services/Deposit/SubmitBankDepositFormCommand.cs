using System;
using System.IO;

namespace TGS.Services.Operator.Services.Deposit
{
    public record SubmitBankDepositFormCommand(
        string PlayerCode,
        Guid FormId,
        string ContentType,
        Stream FileStream,
        Func<string, string> CreatePaymentReturn);
}
