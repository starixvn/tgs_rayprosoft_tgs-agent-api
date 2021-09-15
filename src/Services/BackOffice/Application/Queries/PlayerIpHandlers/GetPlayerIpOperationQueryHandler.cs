using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.PlayerIp;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Queries.PlayerIpHandlers
{
    public class GetPlayerIpOperationQueryHandler : Handler<GetPlayerIpsRequest, PagedResponse<PlayerIpModel>>
    {
        private readonly IPlayerIpService _service;

        public GetPlayerIpOperationQueryHandler(IPlayerIpService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service ?? throw new System.ArgumentNullException(nameof(service));
        }

        public override Task<PagedResponse<PlayerIpModel>> Handle(GetPlayerIpsRequest request,
            CancellationToken cancellationToken)
        {
            var source = _service.GetPlayerIpOperationQueries(request.BrandId);
            return source
                 .GroupBy(x => new
                 {
                     IpAddress = x.IpAddress
                 })
                 .Select(group => new PlayerIpModel
                 {
                     IpAddress = group.Key.IpAddress,
                     MemberCount = source.Where(x => x.IpAddress == group.Key.IpAddress).GroupBy(x => x.PlayerCode).Count()
                 })
                 .OrderByDescending(x => x.MemberCount)
                 .CreatePagedResponseAsync(request);
        }

    }
}
