using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Queries.PlayerHandlers
{
    public class GetPlayersQueryHandler : Handler<GetPlayersRequest, PagedResponse<PlayersModel>>
    {
        private readonly IPlayerService _service;

        public GetPlayersQueryHandler(IPlayerService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        public override Task<PagedResponse<PlayersModel>> Handle(GetPlayersRequest request,
            CancellationToken cancellationToken) =>
            _service.GetPlayerQueries(q => q.Where(ResellerId != null,
                        r => r.Brand.Licensee.ResellerId == ResellerId)
                    .Where(LicenseeId != null,
                        l => l.Brand.LicenseeId == LicenseeId)
                    .Where(BrandId != null, b => b.BrandId == BrandId)
                    .Where(request.BrandId, b => b.BrandId == request.BrandId)
                    .Where(request.Username, x => x.Username == request.Username))
                .Select(x => new PlayersModel
                {
                    PlayerCode = x.PlayerCode, Username = x.Username, VipLevel = x.Vip.Level, Status = x.Status
                }).CreatePagedResponseAsync(request);
    }
}