using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IDepositRepository
    {
        ValueTask CreateDepositFormAsync(BankDepositForm form);

        ValueTask<BankDepositForm> GetDepositFormByIdAsync(Guid id);
    }
}
