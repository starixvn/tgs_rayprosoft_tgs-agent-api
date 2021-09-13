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
    public class GetPlayerIpByUserNameOperationQueryHandler : Handler<GetPlayerIpsByUserNameRequest, PagedResponse<PlayerIpByUserNameModel>>
    {
        private readonly IPlayerIpService _service;

        public GetPlayerIpByUserNameOperationQueryHandler(IPlayerIpService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service ?? throw new System.ArgumentNullException(nameof(service));
        }

        public override Task<PagedResponse<PlayerIpByUserNameModel>> Handle(GetPlayerIpsByUserNameRequest request, CancellationToken cancellationToken)
            => _service
            .GetPlayerIpOperationByUsernameQueries(request.BrandId, request.UserName)
            .GroupBy(x => new
            {
                IpAddress = x.IpAddress
            })
            .Select(group => new PlayerIpByUserNameModel
            {
                IpAddress = group.Key.IpAddress,
                CreatedOn = group.Max(y => y.CreatedOn)
            })
            .OrderByDescending(x => x.CreatedOn)
            .CreatePagedResponseAsync(request);


    }
}
