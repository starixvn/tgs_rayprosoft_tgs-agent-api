using System;
using System.Threading.Tasks;

namespace TGS.Services.Operator.BankDeposit
{
    public record CreateBankDepositFormCommand(
        string PlayerCode,
        Guid BrandId,
        Guid BrandPaymentId,
        string BankAccount,
        decimal Amount)
    {
        public TaskCompletionSource<Guid> ReturnFormIdSource { get; } = new TaskCompletionSource<Guid>();
    }
}
