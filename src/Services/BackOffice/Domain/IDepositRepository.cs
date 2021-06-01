using System;
using System.Threading.Tasks;
using Tgs.Payment;

namespace TGS.Services.BackOffice.Domain
{
    public interface IDepositRepository
    {
        ValueTask<DepositFormDetail> GetDepositFormDetailAsync(Guid formId);
    }
}
