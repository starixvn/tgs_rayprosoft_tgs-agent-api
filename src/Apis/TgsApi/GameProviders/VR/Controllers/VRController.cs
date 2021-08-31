using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameProviders.VR.Services;
using TGS.Core.Models;
using TGS.Shared.Game.Models.VR.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.VR.Controllers
{
    [Route("api/vr")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.VR)]
    public class VRController : ControllerBase
    {
        private readonly IVRGPGameHistoryService _gameHistoryService;

        public VRController(
            IVRGPGameHistoryService gameHistoryService
            )
        {
            _gameHistoryService = gameHistoryService ?? throw new ArgumentNullException(nameof(gameHistoryService));
        }

        [HttpPost("GameRecord")]
        public ValueTask<VRGPGameRecordResponse> GameRecordAsync(VRGPGameRecordRequest request)
            => _gameHistoryService.GameRecordAsync(request);
    }
}