using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;

namespace TGS.Apis.TgsApi.Commons.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public sealed class PluggableController : ControllerBase
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly Func<string, IGameHistory> _historyFactory;
        private readonly Func<string, IGameInitializer> _initializerFactory;
        private readonly Func<string, IPlayerAction> _playerActionFactory;

        public PluggableController(
            Func<string, IGameInitializer> initializerFactory,
            Func<string, IGameHistory> historyFactory,
            Func<string, IPlayerAction> playerActionFactory,
            IGameProviderOperations gameProviderOperations)
        {
            _initializerFactory = initializerFactory;
            _historyFactory = historyFactory;
            _playerActionFactory = playerActionFactory;
            _gameProviderOperations = gameProviderOperations;
        }

        [HttpPost]
        [Route("gameurl")]
        public Task<GetGameUrlResponse> GetGameUrlAsync(GetGameUrlRequest request) =>
            _initializerFactory(request.GameProviderCode).GetGameUrlAsync(request);

        [HttpPost]
        [Route("history/round")]
        public Task<GetRoundHistoryResponse> GetRoundHistory(GetRoundHistoryRequest request) =>
            _historyFactory(request.GameProviderCode).GetRoundHistoryAsync(request);

        [HttpPost]
        [Route("player/kick")]
        public async Task<PlayerKickResponse> KickPlayerAsync(PlayerKickRequest request)
        {
            var gameProviderCode = request.GameProviderCode;
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(gameProviderCode);
            var allowKickPlayer = await _gameProviderOperations.GetAllowKickPlayerAsync(gameProviderId);

            if (!allowKickPlayer)
            {
                throw new NotSupportedException(
                    $"Game Provider '{request.GameProviderCode}' doesn't support to kick player");
            }

            return await _playerActionFactory(gameProviderCode).KickAsync(request);
        }
    }
}