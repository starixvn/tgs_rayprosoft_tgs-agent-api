// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Factories;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.SBOBET.Incoming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Controllers
{
    [Route("api/sbo")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(GameProviderConst.SBOBET)]
    public sealed partial class SBOBETGameController : ControllerBase
    {
        [HttpPost]
        [Route("GetBalance")]
        public async ValueTask<GetBalanceResponse> GetBalanceAsync(
            GetBalanceRequest request,
            [FromServices] IGamePlayActions actions,
            [FromServices] ISBOBETActionModelMapper map)
        {
            var r = await actions.GetBalanceAsync(GameProviderConst.SBOBET, request.PlayerCode)
                .ConfigureAwait(false);

            return map.Response(request.PlayerCode, r);
        }

        [HttpPost]
        [Route("Deduct")]
        public async ValueTask<PlaceBetResponse> PlaceBetAsync(
            PlaceBetRequest request,
            [FromServices] IBetServiceFactory factory)
        {
            var service = factory.Get(request.ProductType);
            return await service.PlaceBetAsync(request).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("Settle")]
        public async ValueTask<SettleBetResponse> SettleBetAsync(
            SettleBetRequest request,
            [FromServices] IBetServiceFactory factory)
        {
            var service = factory.Get(request.ProductType);
            return await service.SettleBetAsync(request).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("Cancel")]
        public async ValueTask<CancelBetResponse> CancelBetAsync(
            CancelBetRequest request,
            [FromServices] IBetServiceFactory factory)
        {
            var service = factory.Get(request.ProductType);
            return await service.CancelBetAsync(request).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("Rollback")]
        public async ValueTask<RollbackResponse> RollbackAsync(
            RollbackRequest request,
            [FromServices] IBetServiceFactory factory)
        {
            var service = factory.Get(request.ProductType);
            return await service.RollbackAsync(request).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("GetBetStatus")]
        public async ValueTask<GetBetStatusResponse> GetBetStatusAsync(
            GetBetStatusRequest request,
            [FromServices] IBetServiceFactory factory)
        {
            var service = factory.Get(request.ProductType);
            return await service.GetBetStatusAsync(request).ConfigureAwait(false);
        }
    }
}