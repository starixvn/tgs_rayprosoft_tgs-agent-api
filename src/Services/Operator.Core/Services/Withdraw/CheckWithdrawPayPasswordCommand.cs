using System.Threading.Tasks;
using System;

namespace TGS.Services.Operator.Services.Withdraw
{
    public record CheckWithdrawPayPasswordCommand(
        string PlayerCode,
        string PayPassword)
    {
        public TaskCompletionSource<bool> ReturnSource { get; } = new TaskCompletionSource<bool>();
    }
}
