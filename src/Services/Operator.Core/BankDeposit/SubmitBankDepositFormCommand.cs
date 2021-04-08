using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Services.Operator.BankDeposit
{
    public record SubmitBankDepositFormCommand(
        string PlayerCode, 
        Guid FormId, 
        string ContentType,
        Stream FileStream);
}
