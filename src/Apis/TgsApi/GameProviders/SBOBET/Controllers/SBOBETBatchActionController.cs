// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Models;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
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
        private static readonly string GameProviderCode = Consts.GameProvider.SBOBET;
        private readonly IGamePlayActions _action;
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ISBOBETActionModelMapper _map;
        private readonly IPlayerOperations _playerOperations;

        public SBOBETBatchActionController(
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations, IGameProviderOperations gameProviderOperations,
            IGamePlayActions action,
            ISBOBETActionModelMapper map)
        {
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _action = action;
            _map = map;
        }

        [Route("transactions/cancel")]
        public async Task<BatchResponse> CancelTransactionsAsync(BatchRequest request)
        {
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(GameProviderCode);
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId);
            var data = await GetPlayerGameProviderConfigurationIdsAsync(gameProviderId, request.Transactions);

            var r = await _action.CancelBatchAsync(_map.Request(gameProviderId, data, autoCloseRound, request));
            return _map.Response(r);
        }

        private async Task<Dictionary<string, Guid>> GetPlayerGameProviderConfigurationIdsAsync(Guid gameProviderId,
            BatchRequestTransaction[] transactions)
        {
            var map = new Dictionary<string, Guid>();

            foreach (var playerCode in transactions.GroupBy(t => t.PlayerCode).Select(g => g.Key))
            {
                if (map.ContainsKey(playerCode))
                {
                    continue;
                }

                var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
                var gameProviderConfigurationId =
                    await _brandOperations.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId);

                map.Add(playerCode, gameProviderConfigurationId);
            }

            return map;
        }
    }
}