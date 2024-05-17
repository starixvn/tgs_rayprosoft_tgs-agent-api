using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Agent.Exceptions;
using TGS.Services.Agent.ValidateCode;
using TGS.Services.Operations;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Agent;

internal class CommitSetAgentEmailCommandService(
    IValidateCodeOperator validateCodeOperator,
    IAgentOperations agentOperations)
    : ICommandService<CommitSetAgentEmailCommand>
{
    private readonly IAgentOperations _agentOperations =
        agentOperations ?? throw new ArgumentNullException(nameof(agentOperations));

    private readonly IValidateCodeOperator _validateCodeOperator =
        validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));

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