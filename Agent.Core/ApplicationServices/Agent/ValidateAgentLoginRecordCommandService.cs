using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Agent;

internal class ValidateAgentLoginRecordCommandService(IAgentLoginRecordRepository loginRecordRepository)
    : ICommandService<ValidateAgentLoginRecordCommand>
{
    private readonly IAgentLoginRecordRepository _loginRecordRepository =
        loginRecordRepository ?? throw new ArgumentNullException(nameof(loginRecordRepository));

    public async ValueTask ExecuteAsync(ValidateAgentLoginRecordCommand command,
        CancellationToken cancellationToken = default)
    {
        var loginRecord = await _loginRecordRepository.GetAgentLoginRecordByIdAsync(command.LoginRecordId)
            .ConfigureAwait(false);

        if (loginRecord is null || loginRecord.ExpiredOn < DateTimeOffset.UtcNow)
            throw new Exception("Agent login token is expired");
    }
}