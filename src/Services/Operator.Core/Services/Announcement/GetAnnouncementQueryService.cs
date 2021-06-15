using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operations;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Announcement
{
    internal class GetAnnouncementQueryService : IGetService<GetAnnouncementQuery, AnnouncementInfo>
    {
        private readonly IBrandOperations _brandOperations;

        public GetAnnouncementQueryService(IBrandOperations brandOperations)
        {
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
        }

        public async ValueTask<AnnouncementInfo> GetAsync(GetAnnouncementQuery query, CancellationToken cancellationToken = default)
        {
            var announcements = await _brandOperations.GetEffectiveAnnouncementsAsync(query.BrandId);

            return new AnnouncementInfo()
            {
                Announcements = announcements.OrderBy(a => a.From)
                    .ThenBy(a => a.To)
                    .Select(a => new Operator.Announcement
                    {
                        Subject = a.Subject,
                        Content = a.Content,
                        From = a.From.ToUnixTimeMilliseconds(),
                        To = a.To.ToUnixTimeMilliseconds()
                    }).ToArray()
            };
        }
    }
}