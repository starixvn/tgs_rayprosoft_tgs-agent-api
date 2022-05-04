using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Agent;

internal class ValidateAgentLoginRecordCommandService : ICommandService<ValidateAgentLoginRecordCommand>
{
    private readonly IAgentRepository _agentRepository;

    public ValidateAgentLoginRecordCommandService(
        IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
    }

    public async ValueTask ExecuteAsync(ValidateAgentLoginRecordCommand command,
        CancellationToken cancellationToken = default)
    {
        var loginRecord = await _agentRepository.GetAgentLoginRecordByIdAsync(command.LoginRecordId)
            .ConfigureAwait(false);

        if (loginRecord is null || loginRecord.ExpiredOn < DateTimeOffset.UtcNow)
        {
            throw new Exception("Agent login token is expired");
        }
    }
}
