using System;
using System.Threading.Tasks;

namespace TGS.Services.Operator.Withdraw
{
    public record CreateWithdrawFormCommand(
        string PlayerCode,
        Guid BrandId,
        Guid BankInfoId,
        decimal Amount)
    {
        public TaskCompletionSource<Guid> ReturnFormIdSource => new TaskCompletionSource<Guid>();
    }
}
