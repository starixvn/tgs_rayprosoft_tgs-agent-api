using System;
using System.Threading.Tasks;

namespace TGS.Services.Agent.Services.Withdraw
{
    public record CreateWithdrawFormCommand(
        Guid AgentId,
        Guid BrandId,
        Guid BankInfoId,
        decimal Amount)
    {
        public TaskCompletionSource<Guid> ReturnFormIdSource { get; } = new TaskCompletionSource<Guid>();
    }
}
