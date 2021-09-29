using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;
using TGS.Services.Agent.Services.Localization;

namespace TGS.Services.Agent.Services.Localization
{
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
}
