using System;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Agent;

namespace TGS.Agent.Repositories;

internal class AgentLoginRecordRepository(ITgsRepository tgsRepository) : IAgentLoginRecordRepository
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<AgentLoginRecordInfo?> GetAgentLoginRecordByIdAsync(Guid loginRecordId)
    {
        var loginRecord = await _tgsRepository.AgentLoginRecords
            .FindAsync(loginRecordId)
            .ConfigureAwait(false);

        return loginRecord == null ?
            null :
            new AgentLoginRecordInfo(
                loginRecord.Id,
                loginRecord.AgentId,
                loginRecord.Status,
                loginRecord.CreatedOn,
                loginRecord.ExpiredOn);
    }
}
