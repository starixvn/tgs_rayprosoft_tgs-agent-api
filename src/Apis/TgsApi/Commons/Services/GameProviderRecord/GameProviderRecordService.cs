using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.Commons.Services.GameProviderRecord
{
    public interface IGameProviderRecordService
    {
        ValueTask<(Player Player, Guid? GameProviderConfigurationId)> GetPlayerInfoAsync(Guid gameProviderId, string playerCode);
    }
    public class GameProviderRecordService : IGameProviderRecordService
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;

        public GameProviderRecordService(
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
        }

        async ValueTask<(Player Player, Guid? GameProviderConfigurationId)> IGameProviderRecordService.GetPlayerInfoAsync(Guid gameProviderId, string playerCode)
        {
            var player = await _playerOperations.GetPlayerByCodeAsync(playerCode).ConfigureAwait(false);
            if (player == null)
            {
                return (null, null);
            }
            var gameProviderConfigurationId = await _brandOperations.GetBrandGameProviderConfigurationIdAsync(player.BrandId, gameProviderId);

            return (player, gameProviderConfigurationId);
        }
    }
}
