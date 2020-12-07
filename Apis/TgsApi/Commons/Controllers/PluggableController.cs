using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Models.Messages.Pluggables;

namespace TGS.Apis.TgsApi.Commons.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public sealed class PluggableController : ControllerBase
    {
        private readonly Func<string, IGameInitializer> _initializerFactory;
        private readonly Func<string, IGameHistory> _historyFactory;
        private readonly Func<string, IPlayerAction> _playerActionFactory;

        public PluggableController(
            Func<string, IGameInitializer> initializerFactory,
            Func<string, IGameHistory> historyFactory,
            Func<string, IPlayerAction> playerActionFactory) 
        {
            _initializerFactory = initializerFactory;
            _historyFactory = historyFactory;
            _playerActionFactory = playerActionFactory;
        }

        [HttpPost]
        [Route("gameurl")]
        public Task<GetGameUrlResponse> GetGameUrlAsync(GetGameUrlRequest request) =>
            _initializerFactory(request.GameProvider.Code).GetGameUrlAsync(request);

        [HttpPost]
        [Route("history/round")]
        public Task<GetRoundHistoryResponse> GetRoundHistory(GetRoundHistoryRequest request) =>
            _historyFactory(request.GameProviderCode).GetRoundHistoryAsync(request);

        [HttpPost]
        [Route("player/kick")]
        public Task<PlayerKickResponse> KickPlayerAsync(PlayerKickRequest request) =>
            _playerActionFactory(request.GameProviderCode).KickAsync(request);
    }
}