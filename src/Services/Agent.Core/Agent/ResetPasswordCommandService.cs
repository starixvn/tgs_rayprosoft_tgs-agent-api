using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Security;
using TGS.Services.Operations;
using TGS.Services.Operations.Models.Services;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Agent
{
    internal class ResetPasswordCommandService : ICommandService<ResetPasswordCommand>
    {
        private readonly IAgentOperations _agentOperations;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public ResetPasswordCommandService(
            IAgentOperations agentOperations,
            IPasswordHandler passwordHandler,
            IValidateCodeOperator validateCodeOperator)
        {
            _agentOperations = agentOperations ?? throw new ArgumentNullException(nameof(agentOperations));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
        }

        public async ValueTask ExecuteAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _validateCodeOperator
                .GetValidateCodeDataAsync<ValidateCodeData>(command.ValidateCode, command.SecurityKey)
                .ConfigureAwait(false);

            if (result == null)
                throw new ValidateCodeNotVerifyException();

            var username = await _agentOperations
                .GetAgentAsync(new Guid(result.Id))
                .ConfigureAwait(false);
            var updatedOn = DateTimeOffset.UtcNow;

            var (newSalt, newPassword) = _passwordHandler.GenerateCredential(command.NewPassword);
            await _agentOperations
                .ChangeAgentPasswordAsync(new ChangeAgentPasswordData()
                {
                    Username = username.Username,
                    Id = new Guid(result.Id),
                    Salt = newSalt,
                    Password = newPassword,
                    UpdatedOn = updatedOn
                })
                .ConfigureAwait(false);
        }
    }
}