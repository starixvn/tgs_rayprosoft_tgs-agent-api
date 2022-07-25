using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Paging;
using TGS.Services.BackOffice.Domain;

namespace TGS.Services.BackOffice.Application.Services
{
    internal class GetAllBrandMaintenanceQueryService : IQueryService<GetAllBrandMaintenanceQuery, BrandMaintenanceInfo>, IGetService<GetAllBrandMaintenanceQuery, int>
    {
        private readonly IBackOfficeRepository _backOfficeRepository;

        public GetAllBrandMaintenanceQueryService(IBackOfficeRepository backOfficeRepository)
        {
            _backOfficeRepository = backOfficeRepository ?? throw new ArgumentNullException(nameof(backOfficeRepository));
        }

        public ValueTask<int> GetAsync(GetAllBrandMaintenanceQuery query, CancellationToken cancellationToken = default)
        {
            var data = _backOfficeRepository.QueryBrandMaintenanceByBrandCodeAsync(query.BrandCode);

            return data.CountAsync();
        }

        public IAsyncEnumerable<BrandMaintenanceInfo> QueryAsync(GetAllBrandMaintenanceQuery query, CancellationToken cancellationToken = default)
        {
            var data = _backOfficeRepository.QueryBrandMaintenanceByBrandCodeAsync(query.BrandCode);

            return data.ApplyOrderBy(query.Paging).ApplyPaging(query.Paging);
        }
    }
}
