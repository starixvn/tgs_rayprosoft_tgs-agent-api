// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Shared.Game.Models.SBOBET.Incoming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Controllers
{
    [Route("api/provider")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(GameProviderConst.SBOBET)]
    public sealed class SBOBETGameController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SBOBETGameController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("GetBalance")]
        public Task<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request) =>
            _mediator.Send(request);

        [Route("Deduct")]
        public Task<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request) =>
            _mediator.Send(request);

        [Route("Settle")]
        public Task<SettleBetResponse> SettleBetAsync(SettleBetRequest request) =>
            _mediator.Send(request);

        [Route("Cancel")]
        public Task<CancelBetResponse> CancelBetAsync(CancelBetRequest request) =>
            _mediator.Send(request);

        [Route("Rollback")]
        public Task<RollbackResponse> RollbackAsync(RollbackRequest request) =>
            _mediator.Send(request);

        [Route("GetBetStatus")]
        public Task<GetBetStatusResponse> GetBetStatusAsync(GetBetStatusRequest request) =>
            _mediator.Send(request);
    }
}