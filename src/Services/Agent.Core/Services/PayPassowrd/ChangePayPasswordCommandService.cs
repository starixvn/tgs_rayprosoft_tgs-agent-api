using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;
using TGS.Core.Models.Exceptions;
using TGS.Services.Agent.Services.Withdraw;

namespace TGS.Services.Agent.Services.PayPassowrd
{
    internal class ChangePayPasswordCommandService : ICommandService<ChangePayPasswordCommand>
    {
        private readonly IAgentRepository _agentRepository;
        private readonly ICommandService<CheckWithdrawPayPasswordCommand> _checkPasswordService;
        private readonly IPasswordHandler _passwordHandler;

        public ChangePayPasswordCommandService(
            ICommandService<CheckWithdrawPayPasswordCommand> checkPasswordService,
            IPasswordHandler passwordHandler,
            IAgentRepository agentRepository)
        {
            _checkPasswordService = checkPasswordService ?? throw new ArgumentNullException(nameof(checkPasswordService));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
            _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
        }

        public async ValueTask ExecuteAsync(ChangePayPasswordCommand command, CancellationToken cancellationToken = default)
        {
            var checkCommand = new CheckWithdrawPayPasswordCommand(
                command.AgentId,
                command.OldPassword);

            await _checkPasswordService.ExecuteAsync(checkCommand).ConfigureAwait(false);

            var checkResult = await checkCommand.ReturnSource.Task.ConfigureAwait(false);

            if (!checkResult)
                throw new OperationNotAllowedException();

            var updatedOn = DateTimeOffset.UtcNow;

            var (newSalt, newPassword) = _passwordHandler.GenerateCredential(command.NewPassword);

            await _agentRepository
                .SetPayPasswordAsync(command.AgentId, newPassword, newSalt, updatedOn)
                .ConfigureAwait(false);
        }
    }
}
