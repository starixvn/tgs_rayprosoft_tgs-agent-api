using System;
using System.Collections.Generic;
using System.Threading;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Services
{
    internal class GetAllGamesByBrandIdQueryService : IQueryService<GetAllGamesByBrandIdQuery, GameData>
    {
        private readonly IBackOfficeRepository _backOfficeRepository;

        public GetAllGamesByBrandIdQueryService(IBackOfficeRepository backOfficeRepository)
        {
            _backOfficeRepository = backOfficeRepository ?? throw new ArgumentNullException(nameof(backOfficeRepository));
        }

        public IAsyncEnumerable<GameData> QueryAsync(
            GetAllGamesByBrandIdQuery query,
            CancellationToken cancellationToken = default)
            => _backOfficeRepository.QueryAllGamesAsync(query.BrandId);
    }
}
