using System;
using System.Threading.Tasks;

namespace TGS.Services.Operator.Deposit
{
    public record CreateDepositFormCommand(
        string PlayerCode,
        Guid BrandId,
        Guid BrandPaymentId,
        decimal Amount,
        string Subject,
        Func<Guid, string?> CreatePaymentSettings)
    {
        public TaskCompletionSource<Guid> ReturnFormIdSource { get; } = new TaskCompletionSource<Guid>();
    }
}
