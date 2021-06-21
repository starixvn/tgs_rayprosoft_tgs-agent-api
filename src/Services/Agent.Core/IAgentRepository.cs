using System;
using System.Threading.Tasks;

namespace TGS.Services.Agent
{
    public interface IAgentRepository
    {
        ValueTask<AgentInfo> GetAgentByIdAsync(Guid id);

        ValueTask SetPayPasswordAsync(Guid agentId, byte[] newPassword, byte[] newSalt, DateTimeOffset updatedOn);
    }
}
