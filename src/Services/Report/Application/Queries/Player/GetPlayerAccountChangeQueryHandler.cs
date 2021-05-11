using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Player;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.Player
{
    public class
        GetPlayerAccountChangeQueryHandler : Handler<GetPlayerAccountChangesRequest,
            PagedResponse<PlayerAccountChangeModel>>
    {
        private readonly ITgsPlayerService _tgsPlayerService;

        public GetPlayerAccountChangeQueryHandler(ITgsPlayerService tgaPlayerService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _tgsPlayerService = tgaPlayerService;
        }

        public override async Task<PagedResponse<PlayerAccountChangeModel>> Handle(
            GetPlayerAccountChangesRequest request,
            CancellationToken cancellationToken)
        {
            var (playerAccountChanges, totalCount) = await _tgsPlayerService.GetPlayerAccountChangeAsync(request);

            return await playerAccountChanges.CreatePagedResponseAsync(totalCount);
        }
    }
}