using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Models.Commons;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.GameProviders.Repositories.Models;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Services
{
    internal class DreamGameService : IDreamGameService
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IRedisPlayerLoginRecordService _playerLoginRecordService;
        private readonly ISettingOperations _settingOperations;
        private readonly ITgsRepository _tgsRepository;

        public DreamGameService(
            ISettingOperations settingOperations,
            IRedisPlayerLoginRecordService playerLoginRecordService,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries,
            ITgsRepository tgsRepository)
        {
            _settingOperations = settingOperations ?? throw new System.ArgumentNullException(nameof(settingOperations));
            _playerLoginRecordService = playerLoginRecordService ?? throw new System.ArgumentNullException(nameof(playerLoginRecordService));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _gameActionQueries = gameActionQueries ?? throw new ArgumentNullException(nameof(gameActionQueries));
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<bool> ExistGameActionExternalIdAsync(Guid gameProviderId, string externalTransactionId)
            => await _tgsRepository.GameActions
                        .AsQueryable()
                        .Where(x => x.ExternalTransactionId == externalTransactionId && x.Round.GameProviderId == gameProviderId)
                .Include(gameAction => gameAction.Round).FirstOrDefaultAsync().ConfigureAwait(false) != null;

        public async ValueTask<bool> GetAutoCloseRoundAsync()
                    => await _gameProviderOperations.GetAutoCloseRoundAsync(await GetGameProviderIdAsync().ConfigureAwait(false)).ConfigureAwait(false);

        public async ValueTask<DreamGameConfiguration> GetDreamGameConfigurationAsync()
            => await _settingOperations.GetValueFromJsonByKeyAsync<DreamGameConfiguration>(SettingConsts.GameProviderConfigurations.DreamGame).ConfigureAwait(false);

        public ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId)
            => _gameActionQueries
                .GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId);

        public async ValueTask<GameActionExternalIds> GetGameActionExternalIdByCancelTransactionAsync(Guid gameProviderId, string playerCode, string externalTransactionId)
                            => await _gameActionQueries
                .GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId)
                .ConfigureAwait(false);

        public async ValueTask<OrderInfo[]> GetGameActionsByExternalRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId)
        {
            var playerCode = (await _tgsRepository.Rounds.AsQueryable()
                        .Where(x => x.GameProviderId == gameProviderId && x.ExternalRoundId == externalRoundId && x.GameId == gameId)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false))?.PlayerCode;
            if (playerCode == null)
            {
                return null;
            }
            var gameActionData = await _gameActionQueries
                 .GetGameActionsByExternalRoundIdAsync(gameProviderId, playerCode, externalRoundId, gameId)
                 .ConfigureAwait(false);

            return gameActionData.GameActions.Select(x => new OrderInfo()
            {
                Username = playerCode,
                TicketId = long.Parse(gameActionData.Round.ExternalRoundId),
                Serial = x.ExternalTransactionId,
                Amount = x.Amount
            }).ToArray();
        }

        public async ValueTask<CachedGame> GetGameAsync()
        {
            var gameProviderId = await GetGameProviderIdAsync().ConfigureAwait(false);
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId).ConfigureAwait(false);
            return games.Where(x => x.Type == GameType.LiveDealerLobby).FirstOrDefault();
        }

        public async ValueTask<Guid> GetGameProviderConfigurationIdAsync(Guid brandId)
        {
            var gameProviderId = await GetGameProviderIdAsync().ConfigureAwait(false);
            var brandGameProviderConfiguration = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId).ConfigureAwait(false);
            return brandGameProviderConfiguration.GameProviderConfigurationId;
        }

        public async ValueTask<Guid> GetGameProviderIdAsync()
            => await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.DreamGame).ConfigureAwait(false);

        public string GetHashMD5(string agentName, string ApiKey)
        {
            using var md5 = MD5.Create();
            var dataByte = Encoding.UTF8.GetBytes($"{agentName}{ApiKey}");
            var hash = md5.ComputeHash(dataByte);
            return hash.ToHexString();
        }

        public async ValueTask<PlayerLoginRecordModel> GetPlayerLoginLastAsync(string playerCode)
                            => await _playerLoginRecordService.GetLastAsync(playerCode).ConfigureAwait(false);

        public async ValueTask<OrderInfo[]> GetUnsettleAsync(Guid gameProviderId)
        {
            var dateTime = DateTimeOffset.UtcNow.AddMinutes(-10);
            var rounds = await _tgsRepository.Rounds.AsQueryable()
                        .Where(x => x.GameProviderId == gameProviderId && x.Status == RoundStatus.Open && x.UpdatedOn.UtcDateTime < dateTime.UtcDateTime)
                        .ToArrayAsync()
                        .ConfigureAwait(false);
            var orderInfos = new List<OrderInfo>();
            foreach (var round in rounds)
            {
                var betAction = (await _gameActionQueries
                .GetGameActionExternalIdsAsync(gameProviderId, round.PlayerCode, round.ExternalRoundId, round.GameId, new[] { GameActionType.PlaceBet })
                .ConfigureAwait(false)).FirstOrDefault();
                orderInfos.Add(new OrderInfo()
                {
                    Username = round.PlayerCode,
                    TicketId = long.Parse(round.ExternalRoundId),
                    Serial = betAction.ExternalTransactionId,
                    Amount = betAction.Amount
                });
            }
            return orderInfos.ToArray();
        }
    }
}
