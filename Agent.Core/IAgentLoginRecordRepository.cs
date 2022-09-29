using System;
using System.Threading.Tasks;

namespace TGS.Services.Agent;

public interface IAgentLoginRecordRepository
{
    ValueTask<AgentLoginRecordInfo?> GetAgentLoginRecordByIdAsync(Guid loginRecordId);
}
