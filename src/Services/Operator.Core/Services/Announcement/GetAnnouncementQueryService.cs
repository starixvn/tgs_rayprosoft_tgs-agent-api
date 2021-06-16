using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using TGS.Services.Operations;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Announcement
{
    internal class GetAnnouncementQueryService : IQueryService<GetAnnouncementQuery, AnnouncementInfo>
    {
        private readonly IBrandOperations _brandOperations;

        public GetAnnouncementQueryService(IBrandOperations brandOperations)
        {
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
        }

        public async IAsyncEnumerable<AnnouncementInfo> QueryAsync(
            GetAnnouncementQuery query,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var announcements = await _brandOperations.GetEffectiveAnnouncementsAsync(query.BrandId).ConfigureAwait(false);

            var items = announcements
                .OrderBy(a => a.From)
                .ThenBy(a => a.To)
                .Select(a => new AnnouncementInfo(
                    a.Subject,
                    a.Content,
                    a.From.ToUnixTimeMilliseconds(),
                    a.To.ToUnixTimeMilliseconds()));

            foreach (var data in items)
            {
                yield return data;

                if (cancellationToken.IsCancellationRequested)
                    yield break;
            }
        }
    }
}