using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Banner
{
    internal class GetBannersQueryService : IQueryService<GetBannersQuery, BannerInfo>
    {
        private readonly ISysRepository _sysRepository;

        public GetBannersQueryService(ISysRepository sysRepository)
        {
            _sysRepository = sysRepository ?? throw new ArgumentNullException(nameof(sysRepository));
        }

        public IAsyncEnumerable<BannerInfo> QueryAsync(GetBannersQuery query, CancellationToken cancellationToken = default)
            => _sysRepository.QueryBrandBannersAsync(query.BrandId).Take(10);
    }
}
