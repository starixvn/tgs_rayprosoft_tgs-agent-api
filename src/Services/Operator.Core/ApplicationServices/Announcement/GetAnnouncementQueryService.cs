using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tgs.ServiceUtility;
using TGS.Services.Operator.ApplicationServices.Announcement;

namespace TGS.Services.Operator.ApplicationServices.Announcement
{
    internal class GetAnnouncementQueryService : IQueryService<GetAnnouncementQuery, AnnouncementInfo>
    {
        private readonly IBrandRepository _brandRepository;

        public GetAnnouncementQueryService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        }

        public IAsyncEnumerable<AnnouncementInfo> QueryAsync(
            GetAnnouncementQuery query,
            CancellationToken cancellationToken = default)
            => _brandRepository
                .QueryAnnouncementInfosAsync(query.BrandId, DateTimeOffset.Now)
                .OrderBy(a => a.DisplayOrder);
    }
}
