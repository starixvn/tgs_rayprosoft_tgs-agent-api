using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.GameOperations.Data;
using TGS.Services.GameOperations.Models.Data;
using TGS.Shared.Game.ApiClients.WGSports;
using TGS.Shared.Game.Models.WGSports.Commons;
using TGS.Shared.Game.Models.WGSports.Enums;
using TGS.Shared.Game.Models.WGSports.Incoming;
using TGS.Shared.Game.Models.WGSports.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.WGSports.Services
{
    public interface IWGSportsGPGameHistoryService
    {
        ValueTask<WGSportsGPGameRecordResponse> GameRecordAsync(WGSportsGPGameRecordRequest source);
    }
    public class WGSportsGPGameHistoryService : IWGSportsGPGameHistoryService
    {
        private readonly IWGSportsApiClient _client;
        private readonly IWGSportsService _service;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IGameRoundsProcessor _gameRoundsProcessor;
        private WGSportsConfiguration _configuration;
        private Guid _gameProviderId;
        private Dictionary<string, Player> _players;
        private Dictionary<string, CachedGame> _gameIds;
        private Dictionary<Guid, Guid> _brandGameProviderConfigurationIds;

        public WGSportsGPGameHistoryService(
            IWGSportsApiClient client,
            IWGSportsService service,
            IJsonSerializationProvider jsonSerializationProvider,
            IGameRoundsProcessor gameRoundsProcessor)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
        }

        async ValueTask<WGSportsGPGameRecordResponse> IWGSportsGPGameHistoryService.GameRecordAsync(WGSportsGPGameRecordRequest source)
        {
            _configuration = await _service.GetConfigurationAsync().ConfigureAwait(false);
            _gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            _players = new Dictionary<string, Player>();
            _gameIds = new Dictionary<string, CachedGame>();
            _brandGameProviderConfigurationIds = new Dictionary<Guid, Guid>();

            var responseUnSettle = await UnSettleAsync(source).ConfigureAwait(false);
            var responseSettle = await SettleAsync(source).ConfigureAwait(false);

            var LastGameRecorTime = responseUnSettle.LastGameRecordTime;
            if (LastGameRecorTime == null)
            {
                LastGameRecorTime = responseSettle.LastGameRecordTime;
            }
            else
            {
                if (responseSettle.LastGameRecordTime != null)
                {
                    LastGameRecorTime = (LastGameRecorTime < responseSettle.LastGameRecordTime) ? responseSettle.LastGameRecordTime : LastGameRecorTime;
                }
            }

            return new WGSportsGPGameRecordResponse()
            {
                Success = true,
                Count = responseUnSettle.Count + responseSettle.Count,
                SuccessCount = responseUnSettle.SuccessCount + responseSettle.SuccessCount,
                FailCount = responseUnSettle.FailCount + responseSettle.FailCount,
                LastGameRecorTime = LastGameRecorTime
            };
        }

        private async ValueTask<(int Count, int SuccessCount, int FailCount, DateTimeOffset? LastGameRecordTime)> UnSettleAsync(WGSportsGPGameRecordRequest source)
        {
            var request = new WGSportsGameRecordRequest()
            {
                GameRecordTime = GameRecordTime.BetDate,
                StartTime = source.StartTime.UtcDateTime.AddHours(-4).ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = source.EndTime.UtcDateTime.AddHours(-4).ToString("yyyy-MM-dd HH:mm:ss")
            };

            var response = await _client.GetGameRecordAsync(_configuration, request).ConfigureAwait(false);
            if (response.ReturnCode.IgnoreCaseNotEquals("1"))
            {
                throw new GameProviderApiErrorException(response.ReturnCode.ToString());
            }

            return await ExecuteRecordAsync(response.BetInfo).ConfigureAwait(false);
        }

        private async ValueTask<(int Count, int SuccessCount, int FailCount, DateTimeOffset? LastGameRecordTime)> SettleAsync(WGSportsGPGameRecordRequest source)
        {
            var request = new WGSportsGameRecordRequest()
            {
                GameRecordTime = GameRecordTime.SettleDate,
                StartTime = source.StartTime.UtcDateTime.AddHours(-4).ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = source.EndTime.UtcDateTime.AddHours(-4).ToString("yyyy-MM-dd HH:mm:ss")
            };

            var response = await _client.GetGameRecordAsync(_configuration, request).ConfigureAwait(false);
            if (response.ReturnCode.IgnoreCaseNotEquals("1"))
            {
                throw new GameProviderApiErrorException(response.ReturnCode.ToString());
            }
            if (response.BetInfo == null || response.BetInfo.Length == 1)
            {
                return (0, 0, 0, null);
            }
            return await ExecuteRecordAsync(response.BetInfo).ConfigureAwait(false);

        }

        private async ValueTask<(int Count, int SuccessCount, int FailCount, DateTimeOffset? LastGameRecordTime)> ExecuteRecordAsync(WGSportsBetInfo[] betInfos)
        {
            var count = 0;
            var successCount = 0;
            var failCount = 0;
            var lastGameRecordTime = default(DateTimeOffset?);
            var sysDateTimeOffset = DateTime.UtcNow;
            for (int i = 0; i < betInfos.Length; i++)
            {
                count++;

                var gameId = default(int?);

                var success = true;
                var gameRecord = betInfos[i];
                #region Get Player
                if (!_players.TryGetValue(gameRecord.Username, out Player player))
                {
                    player = await _service.GetPlayerByCodeAsync(gameRecord.Username).ConfigureAwait(false);
                    _players.Add(gameRecord.Username, player);
                }

                if (!_brandGameProviderConfigurationIds.TryGetValue(player.BrandId, out Guid gameProviderConfigurationId))
                {
                    gameProviderConfigurationId = await _service.GetBrandGameProviderConfigurationIdAsync(player.BrandId, _gameProviderId).ConfigureAwait(false);
                    _brandGameProviderConfigurationIds.Add(player.BrandId, gameProviderConfigurationId);
                }
                #endregion

                #region Get Game
                if (_gameIds.TryGetValue(gameRecord.InitData.FB, out CachedGame game))
                {
                    gameId = game.Id;
                }
                else
                {
                    game = await _service.GetCachedGameAsync(_gameProviderId, gameRecord.InitData.FB).ConfigureAwait(false);

                    if (game != null)
                    {
                        gameId = game.Id;
                        _gameIds.Add(gameRecord.InitData.FB, game);
                    }
                }
                #endregion

                if (player == null || !gameId.HasValue)
                {
                    success = false;
                    failCount++;
                }
                else
                {
                    successCount++;
                }

                var createdOn = new DateTimeOffset(DateTime.Parse(gameRecord.RecordDate), TimeSpan.FromHours(-4));
                DateTimeOffset? externalClosedOn = gameRecord.InitData.Win.IgnoreCaseEquals("0") ? null : new DateTimeOffset(DateTime.Parse(gameRecord.SettleDate), TimeSpan.FromHours(-4));
                await _gameRoundsProcessor.ExecuteAsync(new GameRoundsData()
                {
                    ExternalRoundId = gameRecord.Reference,
                    GameProviderId = _gameProviderId,
                    GameProviderCode = _service.GetGameProviderCode(),
                    PlayerCode = (player == null) ? string.Empty : player.PlayerCode,
                    GameId = gameId.GetValueOrDefault(-1),
                    ExternalId = gameRecord.Reference.ToString(),
                    CurrencyCode = player.DefaultWalletCurrencyCode,
                    Status = gameRecord.InitData.Win.IgnoreCaseEquals("0") ? Core.Models.Enums.RoundStatus.Open : Core.Models.Enums.RoundStatus.Closed,
                    GameProviderConfigurationId = gameProviderConfigurationId,
                    WalletId = (player == null) ? Guid.Empty : player.DefaultWalletId,
                    CreatedOn = sysDateTimeOffset,
                    ClosedOn = gameRecord.InitData.Win.IgnoreCaseEquals("0") ? null : sysDateTimeOffset,
                    UpdatedOn = sysDateTimeOffset,
                    BrandId = (player == null) ? Guid.Empty : player.BrandId,
                    ExternalCreatedOn = createdOn,
                    ExternalClosedOn = externalClosedOn,
                    BetAmount = decimal.Parse(gameRecord.BetAmount),
                    ValidBetAmount = decimal.Parse(gameRecord.ActiveAmount),
                    PlayerWinLoss = gameRecord.InitData.Win.IgnoreCaseEquals("0") ? null : decimal.Parse(gameRecord.ResultScore),
                    Commission = 0,
                    Turnover = decimal.Parse(gameRecord.ActiveAmount),
                    SettleAmount = decimal.Parse(gameRecord.Result),
                    TieBetAdjustment = null,
                    ClientStatus = gameRecord.InitData.Win.IgnoreCaseEquals("0") ? Core.Models.Enums.ClientRoundStatus.Open : _service.GetClientRound(decimal.Parse(gameRecord.Result)),
                    AdditionalData = _jsonSerializationProvider.SerializeToString(gameRecord),
                    Success = success
                });
                lastGameRecordTime = (externalClosedOn.HasValue) ? externalClosedOn : createdOn;
            }

            return (count, successCount, failCount, lastGameRecordTime);
        }
    }
}
