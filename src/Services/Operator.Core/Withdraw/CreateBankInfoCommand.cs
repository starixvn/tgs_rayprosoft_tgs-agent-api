using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Services.Operator.Withdraw
{
    public record CreateBankInfoCommand(
        Guid BrandId,
        string PlayerCode,
        Guid PaymentChannelId,
        string BankType,
        string Detail);
}
