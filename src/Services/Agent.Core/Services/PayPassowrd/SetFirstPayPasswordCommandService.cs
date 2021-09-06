using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Security;
using TGS.Services.Agent.Exceptions;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.PayPassowrd
{
    internal class SetFirstPayPasswordCommandService : ICommandService<SetFirstPayPasswordCommand>
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IPasswordHandler _passwordHandler;

        public SetFirstPayPasswordCommandService(IAgentRepository agentRepository, IPasswordHandler passwordHandler)
        {
            _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
        }

        public async ValueTask ExecuteAsync(SetFirstPayPasswordCommand command,
            CancellationToken cancellationToken = default)
        {
            var agent = await _agentRepository
                .GetAgentByIdAsync(command.AgentId)
                .ConfigureAwait(false);

            var updatedOn = DateTimeOffset.UtcNow;

            if (agent.PayPassword?.Length > 0)
                throw new PayPasswordAlreadySetException();

            var (newSalt, newPassword) = _passwordHandler.GenerateCredential(command.Password);

            await _agentRepository
                .SetPayPasswordAsync(command.AgentId, newPassword, newSalt, updatedOn)
                .ConfigureAwait(false);
        }
    }
}