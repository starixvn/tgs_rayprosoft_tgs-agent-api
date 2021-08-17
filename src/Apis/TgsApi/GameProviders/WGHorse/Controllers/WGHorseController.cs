using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameProviders.WGHorse.Services;
using TGS.Core.Models;
using TGS.Shared.Game.Models.WGHorse.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.WGHorse.Controllers
{
    [Route("api/wgvia")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.WGHorse)]
    public class WGHorseController : ControllerBase
    {
        private readonly IWGHorseGPGameHistoryService _gameHistoryService;

        public WGHorseController(
            IWGHorseGPGameHistoryService gameHistoryService
        )
        {
            _gameHistoryService = gameHistoryService ?? throw new ArgumentNullException(nameof(gameHistoryService));
        }

        [HttpPost("gamerecord")]
        public ValueTask<WGHorseGPGameRecordResponse> GameRecordAsync(WGHorseGPGameRecordRequest source)
       => _gameHistoryService.GameRecordAsync(source);
    }
}
