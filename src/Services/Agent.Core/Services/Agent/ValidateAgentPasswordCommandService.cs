using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Security;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.Agent
{
    internal class ValidateAgentPasswordCommandService : IGetService<ValidateAgentPasswordCommand, bool>
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IPasswordHandler _passwordHandler;

        public ValidateAgentPasswordCommandService(
            IAgentRepository agentRepository,
            IPasswordHandler passwordHandler)
        {
            _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
        }

        public async ValueTask<bool> GetAsync(ValidateAgentPasswordCommand query, CancellationToken cancellationToken = default)
        {
            var agent = await _agentRepository
                .GetAgentByIdAsync(query.AgentId)
                .ConfigureAwait(false);

            if (agent == null)
                return false;

            return _passwordHandler.Verify(
                agent.Password,
                agent.Salt,
                query.Password);
        }
    }
}
