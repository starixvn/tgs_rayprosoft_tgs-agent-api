using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.Services.Agent;

public interface IAgentRepository
{
    ValueTask<AgentInfo?> GetAgentByIdAsync(Guid id);

    IAsyncEnumerable<string> GetAgentDomainsByIdAsync(Guid agentId);

    IAsyncEnumerable<AgentContactInfo> GetAgentsByEmailAsync(Guid brandId, string email);

    IAsyncEnumerable<AgentContactInfo> GetAgentsByPhoneAsync(Guid brandId, string phone);

    ValueTask<AgentStatus> GetAgentStatusByIdAsync(Guid id);

    IAsyncEnumerable<DividendCondition> QueryAgentDividendConditionsAsync(Guid agentId);

    IAsyncEnumerable<AgentPlayerInfo> QueryAgentPlayerInfosAsync(
        Guid agentId,
        string? username,
        DateTimeOffset? beginRegisteredOn,
        DateTimeOffset? endRegisteredOn,
        string? registerHost,
        PlayerStatus? status,
        Guid? vipId);

    IAsyncEnumerable<KeyValuePair<string, string>> QueryAllLocalizationStringsAsync(string cultureCode);

    ValueTask SetPasswordAsync(Guid agentId, byte[] newPassword, byte[] newSalt, DateTimeOffset updatedOn);

    ValueTask SetPayPasswordAsync(Guid agentId, byte[] newPassword, byte[] newSalt, DateTimeOffset updatedOn);
}