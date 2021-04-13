using System;
using System.Threading.Tasks;
using TGS.Services.BackOffice.Domain.Enums;

namespace TGS.Services.BackOffice.Domain
{
    public interface IDepositRepository
    {
        ValueTask SignOffDepositFormAsync(Guid formId, ReviewStatus reviewStatus);
    }
}
