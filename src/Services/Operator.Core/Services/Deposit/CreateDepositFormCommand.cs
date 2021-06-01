using System;
using System.Threading.Tasks;

namespace TGS.Services.Operator.Services.Deposit
{
    public record CreateDepositFormCommand(
        string PlayerCode,
        Guid BrandId,
        Guid BrandPaymentId,
        decimal Amount,
        string Subject,
        Guid? PromotionId,
        Func<Guid, string?> CreatePaymentSettings)
    {
        public TaskCompletionSource<Guid> ReturnFormIdSource { get; } = new TaskCompletionSource<Guid>();
    }
}
