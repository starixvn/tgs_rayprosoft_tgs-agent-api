using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tgs.ServiceUtility;
using TGS.Core.DataServices.Tgs;

namespace TGS.Services.Agent.ApplicationServices.Agent;

internal class CheckAgentExistsQueryService(ITgsRepository tgsRepository) : IGetService<CheckAgentExistsQuery, bool>
{
    private readonly ITgsRepository _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<bool> GetAsync(
        CheckAgentExistsQuery query,
        CancellationToken cancellationToken = default)
        => await _tgsRepository.Agents.AsQueryable()
            .AnyAsync(x => x.BrandId == query.BrandId
                    && x.Username.ToLower().Equals(query.Username.ToLower()),
                cancellationToken);
}
