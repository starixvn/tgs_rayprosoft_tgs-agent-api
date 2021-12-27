using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.Services.Agent
{
    public interface IAgentRepository
    {
        ValueTask<AgentInfo?> GetAgentByIdAsync(Guid id);

        IAsyncEnumerable<string> GetAgentDomainsByIdAsync(Guid agentId);

        IAsyncEnumerable<AgentInfo> GetAgentsByEmailAsync(Guid brandId, string email);

        IAsyncEnumerable<AgentInfo> GetAgentsByPhoneAsync(Guid brandId, string phone);

        ValueTask<AgentStatus> GetAgentStatusByIdAsync(Guid id);

        IAsyncEnumerable<DividendCondition> QueryAgentDividendConditionsAsync(Guid agentId);

        IAsyncEnumerable<KeyValuePair<string, string>> QueryAllLocalizationStringsAsync(string cultureCode);

        ValueTask SetPasswordAsync(Guid agentId, byte[] newPassword, byte[] newSalt, DateTimeOffset updatedOn);

        ValueTask SetPayPasswordAsync(Guid agentId, byte[] newPassword, byte[] newSalt, DateTimeOffset updatedOn);
    }
}
