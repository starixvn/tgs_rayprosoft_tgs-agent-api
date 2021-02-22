// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Core.Models;
using TGS.Shared.Game.Models.SBOBET.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Controllers
{
    [Route("api/batch")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.SBOBET)]
    public sealed class SBOBETBatchActionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SBOBETBatchActionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("transactions/cancel")]
        public Task<BatchResponse> CancelTransactionsAsync(BatchRequest request) =>
            _mediator.Send(request);
    }
}
