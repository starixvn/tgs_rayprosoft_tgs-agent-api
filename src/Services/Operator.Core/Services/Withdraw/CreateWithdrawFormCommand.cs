using System;
using System.Threading.Tasks;

namespace TGS.Services.Operator.Services.Withdraw
{
    public record CreateWithdrawFormCommand(
        string PlayerCode,
        Guid BrandId,
        Guid BankInfoId,
        decimal Amount)
    {
        public TaskCompletionSource<Guid> ReturnFormIdSource { get; } = new TaskCompletionSource<Guid>();
    }
}
