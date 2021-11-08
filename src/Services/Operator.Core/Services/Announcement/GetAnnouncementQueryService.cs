using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.Services.Announcement
{
    internal class GetAnnouncementQueryService : IQueryService<GetAnnouncementQuery, AnnouncementInfo>
    {
        private readonly ISysRepository _sysRepository;

        public GetAnnouncementQueryService(ISysRepository sysRepository)
        {
            _sysRepository = sysRepository ?? throw new ArgumentNullException(nameof(sysRepository));
        }

        public IAsyncEnumerable<AnnouncementInfo> QueryAsync(
            GetAnnouncementQuery query,
            CancellationToken cancellationToken = default)
            => _sysRepository
                .QueryAnnouncementInfosAsync(query.BrandId, DateTimeOffset.Now)
                .OrderBy(a => a.DisplayOrder);
    }
}
