using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Core.Models;
using TGS.Shared.Game.Models.Habanero.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Habanero.Controllers
{
    [Route("api/provider")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.Habanero)]
    public sealed class HabaneroGameController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HabaneroGameController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost, Route("authorize")]
        public Task<AuthorizePlayerResponse> AuthorizePlayerAsync(AuthorizePlayerRequest request) =>
            _mediator.Send(request);
    }
}
