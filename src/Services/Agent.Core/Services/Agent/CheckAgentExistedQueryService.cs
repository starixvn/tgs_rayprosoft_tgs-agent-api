using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.Agent
{
    internal class CheckAgentExistedQueryService : IGetService<CheckAgentExistedQuery, bool>
    {
        private readonly ITgsRepository _tgsRepository;

        public CheckAgentExistedQueryService(
            ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<bool> GetAsync(
            CheckAgentExistedQuery query,
            CancellationToken cancellationToken = default)
            => await _tgsRepository.Agents.AsQueryable()
                .AnyAsync(x => x.BrandId == query.BrandId
                        && EF.Functions.ILike(x.Username, query.Username),
                    cancellationToken);
    }
}