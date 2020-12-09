using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.DataService;
using TGS.Services.BackOffice.Application.Models.BrandGameProviderGame;

namespace TGS.Services.BackOffice.Application.Queries.BrandGameProviderGameHandlers
{
    public class GetGameQueryHandler : Handler<GetGamesRequest,
        PagedResponse<GameModel>>
    {
        private readonly IBrandGameProviderGameService _service;

        public GetGameQueryHandler(IBrandGameProviderGameService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        public override async Task<PagedResponse<GameModel>> Handle(
            GetGamesRequest request,
            CancellationToken cancellationToken)
        {
            return new();
        }
    }
}