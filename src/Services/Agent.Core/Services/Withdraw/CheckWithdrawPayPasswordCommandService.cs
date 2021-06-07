using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Security;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.Withdraw
{
    internal class CheckWithdrawPayPasswordCommandService : ICommandService<CheckWithdrawPayPasswordCommand>
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IPasswordHandler _passwordHandler;

        public CheckWithdrawPayPasswordCommandService(
            IAgentRepository agentRepository,
            IPasswordHandler passwordHandler)
        {
            _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
        }

        public async ValueTask ExecuteAsync(CheckWithdrawPayPasswordCommand command, CancellationToken cancellationToken = default)
        {
            var agentData = await _agentRepository
                .GetAgentByIdAsync(command.AgentId)
                .ConfigureAwait(false);

            var isVaild = _passwordHandler.Verify(
                agentData.PayPassword,
                agentData.PaySalt,
                command.PayPassword);

            command.ReturnSource.SetResult(isVaild);
        }
    }
}
