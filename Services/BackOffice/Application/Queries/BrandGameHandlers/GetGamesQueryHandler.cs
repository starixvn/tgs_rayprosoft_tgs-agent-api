using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.BrandGame;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Queries.BrandGameHandlers
{
    public class GetGamesQueryHandler : Handler<GetBrandGamesRequest,
        PagedResponse<BrandGameModel>>
    {
        private readonly IBrandGameService _service;

        public GetGamesQueryHandler(IBrandGameService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        public override Task<PagedResponse<BrandGameModel>> Handle(
            GetBrandGamesRequest request,
            CancellationToken cancellationToken)
        {
            return _service.GetBrandGamesAsync()
                .Where(request.BrandId, x => x.BrandId == request.BrandId)
                .Where(ResellerId != null, r => r.Brand.Licensee.ResellerId == ResellerId)
                .Where(LicenseeId != null, l => l.Brand.LicenseeId == LicenseeId)
                .Where(BrandId != null, b => b.BrandId == BrandId)
                .Where(request.GameProviderId, x => x.Game.GameProviderId == request.GameProviderId)
                .Where(request.GameType, x => x.Game.Type == request.GameType)
                .Where(request.Keyword, x => x.Game.GameProvider.Name.Contains(request.Keyword) ||
                                             x.Game.Name.Contains(request.Keyword))
                .Select(x => new BrandGameModel
                {
                    Id = x.Id,
                    GameId = x.GameId,
                    GameProviderName = x.Game.GameProvider.Name,
                    Status = x.Status,
                    GameName = x.Game.Name,
                    GameType = x.Game.Type,
                    Platform = x.Game.PlatformType,
                    IsHot = x.Tag.IsHot(),
                    IsNew = x.Tag.IsNew(),
                    IsRecommend = x.Tag.IsRecommend(),
                    DisplayOrder = x.DisplayOrder
                }).CreatePagedResponseAsync(request);
        }
    }
}