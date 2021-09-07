using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.Services.Agent
{
    public interface IAgentRepository
    {
        ValueTask<AgentInfo> GetAgentByIdAsync(Guid id);
        ValueTask<AgentStatus> GetAgentStatusByIdAsync(Guid id);

        ValueTask SetPayPasswordAsync(Guid agentId, byte[] newPassword, byte[] newSalt, DateTimeOffset updatedOn);
    }
}