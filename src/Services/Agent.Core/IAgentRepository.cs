using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Paging;

namespace TGS.Services.Agent;

public interface IAgentRepository
{
    ValueTask<AgentInfo?> GetAgentByIdAsync(Guid id);

    IAsyncEnumerable<string> GetAgentDomainsByIdAsync(Guid agentId);

    IAsyncEnumerable<AgentInfo> GetAgentsByEmailAsync(Guid brandId, string email);

    IAsyncEnumerable<AgentInfo> GetAgentsByPhoneAsync(Guid brandId, string phone);

    ValueTask<AgentStatus> GetAgentStatusByIdAsync(Guid id);

    IAsyncEnumerable<DividendCondition> QueryAgentDividendConditionsAsync(Guid agentId);

    IAsyncEnumerable<AgentPlayerInfo> QueryAgentPlayerInfosAsync(
        Guid agentId,
        string? username,
        DateTimeOffset? beginRegisteredOn,
        DateTimeOffset? endRegisteredOn,
        string? registerHost,
        PlayerStatus? status,
        Guid? vipId,
        Paging<AgentPlayerInfo> paging);

    ValueTask<int> QueryAgentPlayerInfosTotalCountAsync(
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
