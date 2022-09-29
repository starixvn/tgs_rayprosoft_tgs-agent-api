using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Localization;

internal class GetAllLocalizationStringsQueryService
    : IGetService<GetAllLocalizationStringsQuery, IDictionary<string, string>>
{
    private readonly IAgentRepository _agentRepository;

    public GetAllLocalizationStringsQueryService(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
    }

    public async ValueTask<IDictionary<string, string>> GetAsync(
        GetAllLocalizationStringsQuery query,
        CancellationToken cancellationToken = default)
    {
        var allI18N = _agentRepository.QueryAllLocalizationStringsAsync(query.CultureCode);

        return await allI18N
            .ToDictionaryAsync(
                p => p.Key,
                p => p.Value,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
