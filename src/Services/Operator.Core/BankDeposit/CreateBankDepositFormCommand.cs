using System;
using System.Threading.Tasks;

namespace TGS.Services.Operator.BankDeposit
{
    public record CreateBankDepositFormCommand(
        string PlayerCode,
        Guid BrandId,
        Guid BrandPaymentId,
        decimal Amount,
        string? ApplyInfo)
    {
        public TaskCompletionSource<Guid> ReturnFormIdSource { get; } = new TaskCompletionSource<Guid>();
    }
}
