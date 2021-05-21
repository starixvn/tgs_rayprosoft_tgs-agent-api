using System;
using System.Threading.Tasks;
using TGS.Services.Payment;

namespace TGS.Services.BackOffice.Domain
{
    public interface IDepositRepository
    {
        ValueTask<DepositFormDetail> GetDepositFormDetailAsync(Guid formId);
    }
}
