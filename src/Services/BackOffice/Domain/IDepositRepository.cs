using System;
using System.Threading.Tasks;
using Tgs.Payment;

namespace TGS.Services.BackOffice.Domain
{
    public interface IDepositRepository
    {
        ValueTask<DepositFormInfo> GetDepositFormDetailAsync(Guid formId);
    }
}
