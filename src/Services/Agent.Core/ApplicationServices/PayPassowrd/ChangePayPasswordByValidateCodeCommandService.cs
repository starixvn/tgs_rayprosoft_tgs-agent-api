using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;
using TGS.Services.Agent.Exceptions;
using TGS.Services.Agent.ValidateCode;

namespace TGS.Services.Agent.ApplicationServices.PayPassowrd
{
    internal class ChangePayPasswordByValidateCodeCommandService : ICommandService<ChangePayPasswordByValidateCodeCommand>
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public ChangePayPasswordByValidateCodeCommandService(
            IAgentRepository agentRepository,
            IPasswordHandler passwordHandler,
            IValidateCodeOperator validateCodeOperator)
        {
            _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));

            _validateCodeOperator =
                validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
        }

        public async ValueTask ExecuteAsync(
            ChangePayPasswordByValidateCodeCommand command,
            CancellationToken cancellationToken = default)
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
