using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.Services.Agent
{
    public interface IAgentRepository
    {
        ValueTask<AgentInfo> GetAgentByIdAsync(Guid id);

        ValueTask<AgentStatus> GetAgentStatusByIdAsync(Guid id);

        IAsyncEnumerable<KeyValuePair<string, string>> QueryAllLocalizationStringsAsync(string cultureCode);

        ValueTask SetPayPasswordAsync(Guid agentId, byte[] newPassword, byte[] newSalt, DateTimeOffset updatedOn);
    }
}
