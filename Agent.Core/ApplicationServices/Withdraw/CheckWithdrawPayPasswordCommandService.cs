using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Withdraw;

internal class CheckWithdrawPayPasswordCommandService(
    IAgentRepository agentRepository,
    IPasswordHandler passwordHandler)
    : ICommandService<CheckWithdrawPayPasswordCommand>
{
    private readonly IAgentRepository _agentRepository =
        agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));

    private readonly IPasswordHandler _passwordHandler =
        passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));

    public async ValueTask ExecuteAsync(
        CheckWithdrawPayPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var agentData = await _agentRepository
            .GetAgentByIdAsync(command.AgentId)
            .ConfigureAwait(false);

        var isVaild = _passwordHandler.Verify(
            agentData!.PayPassword,
            agentData.PaySalt,
            command.PayPassword);

        command.ReturnSource.SetResult(isVaild);
    }
}