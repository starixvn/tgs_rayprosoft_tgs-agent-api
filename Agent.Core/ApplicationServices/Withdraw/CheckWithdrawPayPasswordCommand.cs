using System;
using System.Threading.Tasks;

namespace TGS.Services.Agent.ApplicationServices.Withdraw;

public record CheckWithdrawPayPasswordCommand(
    Guid AgentId,
    string PayPassword)
{
    public TaskCompletionSource<bool> ReturnSource { get; } = new();
}