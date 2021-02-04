using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operations;

namespace TGS.Apis.OperatorApi.Handlers.Announcement
{
    public sealed class GetAnnouncementsHandler : IRequestHandler<GetAnnouncementsRequest, GetAnnouncementsResponse>
    {
        private readonly IBrandOperations _brandOperations;
        private readonly HttpContext _httpContext;

        public GetAnnouncementsHandler(
            IHttpContextAccessor httpContextAccessor,
            IBrandOperations brandOperations)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _brandOperations = brandOperations;
        }

        async Task<GetAnnouncementsResponse> IRequestHandler<GetAnnouncementsRequest, GetAnnouncementsResponse>.Handle(
            GetAnnouncementsRequest request, CancellationToken cancellationToken)
        {
            var brandId = _httpContext.Items.GetBrandId();

            var announcements = await _brandOperations.GetEffectiveAnnouncementsAsync(brandId);

            return new()
            {
                Announcements = announcements.OrderBy(a => a.From)
                    .ThenBy(a => a.To)
                    .Select(a => new Models.Messages.Announcement
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