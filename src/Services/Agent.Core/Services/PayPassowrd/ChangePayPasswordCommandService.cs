using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Security;
using TGS.Services.Agent.ValidateCode;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.PayPassowrd
{
    internal class ChangePayPasswordCommandService : ICommandService<ChangePayPasswordCommand>
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public ChangePayPasswordCommandService(
            IAgentRepository agentRepository,
            IPasswordHandler passwordHandler,
            IValidateCodeOperator validateCodeOperator)
        {
            _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
        }

        public async ValueTask ExecuteAsync(ChangePayPasswordCommand command, CancellationToken cancellationToken = default)
        {
            var agent = await _agentRepository
                .GetAgentByIdAsync(command.AgentId)
                .ConfigureAwait(false);

            var result = await _validateCodeOperator
                .GetValidateCodeDataAsync<ValidateCodeData>(command.ValidateCode, command.SecurityKey)
                .ConfigureAwait(false);

            if (result == null)
                throw new ValidateCodeNotVerifyException();

            var updatedOn = DateTimeOffset.UtcNow;

            var (newSalt, newPassword) = _passwordHandler.GenerateCredential(command.NewPassword);
            await _agentRepository
                .SetPayPasswordAsync(command.AgentId, newPassword, newSalt, updatedOn)
                .ConfigureAwait(false);
        }
    }
}
