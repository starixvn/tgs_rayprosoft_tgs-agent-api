using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.Services.Agent
{
    internal class CheckAgentExistsQueryService : IGetService<CheckAgentExistsQuery, bool>
    {
        private readonly ITgsRepository _tgsRepository;

        public CheckAgentExistsQueryService(
            ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<bool> GetAsync(
            CheckAgentExistsQuery query,
            CancellationToken cancellationToken = default)
            => await _tgsRepository.Agents.AsQueryable()
                .AnyAsync(x => x.BrandId == query.BrandId
                        && x.Username.ToLower().Equals(query.Username.ToLower()),
                    cancellationToken);
    }
}