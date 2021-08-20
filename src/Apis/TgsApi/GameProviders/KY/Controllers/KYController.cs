using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameProviders.KY.Services;
using TGS.Core.Models;
using TGS.Shared.Game.Models.KY.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.KY.Controllers
{
    [Route("api/ky")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.KY)]
    public class KYController : ControllerBase
    {
        private readonly IKYGPGameHistoryService _gameHistoryService;

        public KYController(
            IKYGPGameHistoryService gameHistoryService
            )
        {
            _gameHistoryService = gameHistoryService ?? throw new ArgumentNullException(nameof(gameHistoryService));
        }

        [HttpPost("GameRecord")]
        public ValueTask<KYGPGameRecordResponse> GameRecordAsync(KYGPGameRecordRequest source)
            => _gameHistoryService.GameRecordAsync(source);
    }
}
