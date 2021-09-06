using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Agent.Exceptions;
using TGS.Services.Agent.ValidateCode;
using TGS.Services.Operations;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.Agent
{
    internal class CommitSetAgentEmailCommandService : ICommandService<CommitSetAgentEmailCommand>
    {
        private readonly IAgentOperations _agentOperations;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public CommitSetAgentEmailCommandService(
            IValidateCodeOperator validateCodeOperator,
            IAgentOperations agentOperations)
        {
            _validateCodeOperator =
                validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
            _agentOperations = agentOperations ?? throw new ArgumentNullException(nameof(agentOperations));
        }

        public async ValueTask ExecuteAsync(CommitSetAgentEmailCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _validateCodeOperator
                .GetValidateCodeDataAsync<EmailValidateData>(command.ValidateCode, command.SecurityKey)
                .ConfigureAwait(false);

            if (result == null)
                throw new ValidateCodeNotVerifyException();

            var updatedOn = DateTimeOffset.UtcNow;

            await _agentOperations
                .SetEmailAsync(command.AgentId, result.Email, updatedOn)
                .ConfigureAwait(false);
        }
    }
}