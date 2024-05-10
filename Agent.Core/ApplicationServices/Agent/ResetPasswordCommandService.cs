using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;
using TGS.Services.Agent.Exceptions;
using TGS.Services.Agent.ValidateCode;
using TGS.Services.Operations;
using TGS.Services.Operations.Models.Services;

namespace TGS.Services.Agent.ApplicationServices.Agent;

internal class ResetPasswordCommandService(
    IAgentOperations agentOperations,
    IPasswordHandler passwordHandler,
    IValidateCodeOperator validateCodeOperator)
    : ICommandService<ResetPasswordCommand>
{
    private readonly IAgentOperations _agentOperations = agentOperations ?? throw new ArgumentNullException(nameof(agentOperations));
    private readonly IPasswordHandler _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
    private readonly IValidateCodeOperator _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));

    public async ValueTask ExecuteAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _validateCodeOperator
            .GetValidateCodeDataAsync<ValidateCodeData>(command.ValidateCode, command.SecurityKey)
            .ConfigureAwait(false);

        if (result == null)
            throw new ValidateCodeNotVerifyException();

        var agentId = new Guid(result.Id);

        var username = await _agentOperations
            .GetAgentAsync(agentId)
            .ConfigureAwait(false);
        var updatedOn = DateTimeOffset.UtcNow;

        var (newSalt, newPassword) = _passwordHandler.GenerateCredential(command.NewPassword);

        await _agentOperations
            .ChangeAgentPasswordAsync(new ChangeAgentPasswordData
            {
                Username = username.Username,
                Id = agentId,
                Salt = newSalt,
                Password = newPassword,
                UpdatedOn = updatedOn
            })
            .ConfigureAwait(false);
    }
}
