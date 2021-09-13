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
    public class GetPlayerIpByIpOperationQueryHandler : Handler<GetPlayerIpsByIpRequest, PagedResponse<PlayerIpByIpModel>>
    {
        private readonly IPlayerIpService _service;

        public GetPlayerIpByIpOperationQueryHandler(IPlayerIpService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service ?? throw new System.ArgumentNullException(nameof(service));
        }

        public override Task<PagedResponse<PlayerIpByIpModel>> Handle(GetPlayerIpsByIpRequest request, CancellationToken cancellationToken)
            => _service
            .GetPlayerIpOperationByIpQueries(request.BrandId, request.IpAddress)
            .GroupBy(x => new
            {
                PlayerCode = x.Player.PlayerCode,
                Username = x.Player.Username

            })
            .Select(group => new PlayerIpByIpModel
            {
                PlayerCode = group.Key.PlayerCode,
                Username = group.Key.Username,
                CreatedOn = group.Max(y => y.CreatedOn)
            })
            .OrderBy(x => x.CreatedOn)
            .CreatePagedResponseAsync(request);


    }
}
