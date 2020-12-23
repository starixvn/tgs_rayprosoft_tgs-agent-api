using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Services.BackOffice.Application.Models.BrandGame;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Queries.BrandGameHandlers
{
    public class GetGameQueryHandler : Handler<GetBrandGameRequest,
        GetBrandGameResponse>
    {
        private readonly IBrandGameService _service;

        public GetGameQueryHandler(IBrandGameService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        public override async Task<GetBrandGameResponse> Handle(
            GetBrandGameRequest request,
            CancellationToken cancellationToken)
        {
            var brandGame = await _service.GetBrandGameAsync(x => x.Id == request.Id);
            if (brandGame == null)
            {
                return CreateInvalidResponse<GetBrandGameResponse>(BrandGame.NotFound);
            }

            return new GetBrandGameResponse
            {
                Valid = true,
                BrandGame = new BrandGameResponseModel
                {
                    Id = brandGame.Id,
                    GameId = brandGame.GameId,
                    GameName = brandGame.Game.Name,
                    Status = brandGame.Status,
                    GameType = brandGame.Game.Type,
                    GameProviderName = brandGame.Game.GameProvider.Name,
                    Platform = brandGame.Game.PlatformType,
                    SupportHtml5 = brandGame.Game.Html5,
                    EffectiveBettingProportion = brandGame.EffectiveBettingPercentage,
                    Rebate = brandGame.Rebate
                }
            };
        }
    }
}