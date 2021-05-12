using System;
using System.Threading.Tasks;

namespace TGS.Services.Operator.GameBalance
{
    public record SubmitGameBalanceFundOutCommand(
        string PlayerCode,
        Guid BrandId,
        Guid BrandPaymentId,
        decimal Amount,
        string Subject)
    {
        public TaskCompletionSource<Guid> ReturnFormIdSource { get; } = new TaskCompletionSource<Guid>();
    }
}
