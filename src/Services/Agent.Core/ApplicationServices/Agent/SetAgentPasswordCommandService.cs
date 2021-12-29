using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Agent
{
    internal class SetAgentPasswordCommandService : ICommandService<SetAgentPasswordCommand>
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IPasswordHandler _passwordHandler;

        public SetAgentPasswordCommandService(
            IPasswordHandler passwordHandler,
            IAgentRepository agentRepository)
        {
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
            _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
        }

        public ValueTask ExecuteAsync(SetAgentPasswordCommand command, CancellationToken cancellationToken = default)
        {
            var (newSalt, newPassword) = _passwordHandler.GenerateCredential(command.NewPassword);

            return _agentRepository.SetPasswordAsync(command.AgentId, newPassword, newSalt, DateTimeOffset.UtcNow);
        }
    }
}
