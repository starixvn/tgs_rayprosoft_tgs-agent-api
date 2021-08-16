using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameProviders.WGSports.Services;
using TGS.Core.Models;
using TGS.Shared.Game.Models.WGSports.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.WGSports.Controllers
{
    [Route("api/wgfm")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.WGSports)]
    public class WGSportsController : ControllerBase
    {
        private readonly IWGSportsGPGameHistoryService _gameHistoryService;

        public WGSportsController(
            IWGSportsGPGameHistoryService gameHistoryService
        )
        {
            _gameHistoryService = gameHistoryService ?? throw new ArgumentNullException(nameof(gameHistoryService));
        }

        [HttpPost("gamerecord")]
        public ValueTask<WGSportsGPGameRecordResponse> GameRecordAsync(WGSportsGPGameRecordRequest source)
        => _gameHistoryService.GameRecordAsync(source);

    }
}
