using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Models;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.SBOBET.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Handlers
{
    public sealed class BatchCancelHandler : IRequestHandler<BatchRequest, BatchResponse>
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGamePlayActions _action;
        private readonly ISBOBETGameControllerMapper _map;

        private const string GameProviderCode = Consts.GameProvider.SBOBET;

        public BatchCancelHandler(
            IPlayerOperations playerOperations,
            
            IBrandOperations brandOperations,IGameProviderOperations gameProviderOperations,
            IGamePlayActions action,
            ISBOBETGameControllerMapper map)
        {
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _action = action;
            _map = map;
        }

        async Task<BatchResponse> IRequestHandler<BatchRequest, BatchResponse>.Handle(BatchRequest request, CancellationToken cancellationToken)
        {
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(GameProviderCode);
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId);
            var data = await GetPlayerGameProviderConfigurationIdsAsync(gameProviderId, request.Transactions);

            var r = await _action.CancelBatchAsync(_map.Request(gameProviderId, data, autoCloseRound, request));
            return _map.Response(r);
        }

        private async Task<Dictionary<string, Guid>> GetPlayerGameProviderConfigurationIdsAsync(Guid gameProviderId, BatchRequestTransaction[] transactions)
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
