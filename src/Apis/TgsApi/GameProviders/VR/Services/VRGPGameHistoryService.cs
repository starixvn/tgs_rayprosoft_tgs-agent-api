using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.GameProviderRecord;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings.VR;
using TGS.Core.Models.Services.Caches;
using TGS.Services.GameOperations.Data;
using TGS.Services.GameOperations.Models.Data;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;
using TGS.Shared.Game.ApiClients.VR;
using TGS.Shared.Game.Models.VR.Incoming;
using TGS.Shared.Game.Models.VR.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.VR.Services
{
    public interface IVRGPGameHistoryService
    {
        ValueTask<VRGPGameRecordResponse> GameRecordAsync(VRGPGameRecordRequest source);
    }
    public class VRGPGameHistoryService : IVRGPGameHistoryService
    {
        private readonly IVRApiClient _client;
        private readonly IVRService _service;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IGameRoundsProcessor _gameRoundsProcessor;
        private readonly IGameProviderRecordService _gameProviderRecordService;
        private DateTimeOffset? _lastGameRecordTime;
        private Dictionary<string, (Player Player, Guid? GameProviderConfigurationId)> _players;
        private Dictionary<int, CachedGame> _games;
        private int _count;
        private int _successCount;
        private int _failCount;

        public VRGPGameHistoryService(
              IVRApiClient client,
              IVRService service,
              IJsonSerializationProvider jsonSerializationProvider,
              IGameRoundsProcessor gameRoundsProcessor,
              IGameProviderRecordService gameProviderRecordService)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
            _gameProviderRecordService = gameProviderRecordService ?? throw new ArgumentNullException(nameof(gameProviderRecordService));
        }

        async ValueTask<VRGPGameRecordResponse> IVRGPGameHistoryService.GameRecordAsync(VRGPGameRecordRequest source)
        {
            var brandMerchant = await _service.GetBrandMerchant(source.BrandCode).ConfigureAwait(false);
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            if (brandMerchant == null)
            {
                return new VRGPGameRecordResponse { Success = false };
            }


            _players = new Dictionary<string, (Player Player, Guid? GameProviderConfigurationId)>();
            _games = new Dictionary<int, CachedGame>();
            _count = 0;
            _successCount = 0;
            _failCount = 0;
            _lastGameRecordTime = default(DateTimeOffset?);
            var unsettled = await GetGameRoundAsync(source, brandMerchant, gameProviderId, false);
            if (!unsettled)
            {
                return new VRGPGameRecordResponse
                {
                    Success = false,
                    SuccessCount = _successCount,
                    FailCount = _failCount,
                    Count = _count
                };
            }

            var settled = await GetGameRoundAsync(source, brandMerchant, gameProviderId, true);
            if (!settled)
            {
                return new VRGPGameRecordResponse
                {
                    Success = false,
                    LastGameRecorTime = _lastGameRecordTime,
                    SuccessCount = _successCount,
                    FailCount = _failCount,
                    Count = _count
                };
            }

            return new VRGPGameRecordResponse
            {
                Success = true,
                LastGameRecorTime = _lastGameRecordTime,
                SuccessCount = _successCount,
                FailCount = _failCount,
                Count = _count
            };
        }

        /// <summary>
        /// Gets the game round asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="brandMerchant">The brand merchant.</param>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <param name="isUpdateTime">if set to <c>true</c> [is update time].</param>
        /// <returns></returns>
        private async Task<bool> GetGameRoundAsync(VRGPGameRecordRequest request, VRBrandMerchant brandMerchant, Guid gameProviderId, bool isUpdateTime)
        {
            var page = 0;
            var rows = 500;
            while (true)
            {
                var data = new VRGameRecordRequest
                {
                    StartTime = request.StartTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    EndTime = request.EndTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    IsUpdateTime = isUpdateTime,
                    Page = page,
                    Rows = rows
                };

                var response = await _client.GetGameRecordAsync(brandMerchant, data);

                if (response.Records.Length == 0)
                {
                    return true;
                }

                page++;
                if (response.Records.Length > 0)
                {
                    var sysDateTimeOffset = DateTimeOffset.UtcNow;


                    foreach (var recordBet in response.Records)
                    {
                        _count++;
                        var success = true;
                        var playerCode = recordBet.PlayerName;

                        if (isUpdateTime)
                        {
                            _lastGameRecordTime = recordBet.UpdateTime;
                        }

                        if (!_players.TryGetValue(playerCode, out (Player Player, Guid? GameProviderConfigurationId) playerInfo))
                        {
                            playerInfo = await _gameProviderRecordService.GetPlayerInfoAsync(gameProviderId, playerCode).ConfigureAwait(false);
                            _players.Add(playerCode, playerInfo);
                        }


                        if (!_games.TryGetValue(recordBet.ChannelId, out CachedGame game))
                        {
                            game = await _service.GetCachedGameAsync(gameProviderId, recordBet.ChannelId.ToString()).ConfigureAwait(false);
                            _games.Add(recordBet.ChannelId, game);
                        }

                        if (playerInfo.Player == null || game == null)
                        {
                            success = false;
                            _failCount++;
                        }
                        else
                        {
                            _successCount++;
                        }

                        if (playerInfo.Player == null || game == null)
                        {
                            success = false;
                            _failCount++;
                        }
                        else
                        {
                            _successCount++;
                        }

                        await _gameRoundsProcessor.ExecuteAsync(new GameRoundsData
                        {
                            ExternalRoundId = recordBet.SerialNumber,
                            GameProviderId = gameProviderId,
                            GameProviderCode = _service.GetGameProviderCode(),
                            PlayerCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.PlayerCode,
                            GameId = (game == null) ? -1 : game.Id,
                            ExternalId = recordBet.ChannelId.ToString(),
                            CurrencyCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.DefaultWalletCurrencyCode,
                            Status = _service.GetRoundStatus(recordBet.State),
                            GameProviderConfigurationId = playerInfo.GameProviderConfigurationId.GetValueOrDefault(),
                            WalletId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.DefaultWalletId,
                            CreatedOn = sysDateTimeOffset,
                            ClosedOn = sysDateTimeOffset,
                            UpdatedOn = sysDateTimeOffset,
                            BrandId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.BrandId,
                            ExternalCreatedOn = recordBet.CreateTime,
                            ExternalClosedOn = recordBet.UpdateTime,
                            BetAmount = recordBet.Cost,
                            ValidBetAmount = recordBet.State == 1 ? 0 : recordBet.Cost,
                            PlayerWinLoss =
                               recordBet.State == 1
                                   ? 0
                                   :
                                   _service.GetRoundStatus(recordBet.State) == RoundStatus.Closed
                                       ?
                                       recordBet.PlayerPrize - recordBet.Cost
                                       : null,
                            Commission = 0,
                            Turnover = recordBet.State == 1 ? 0 : recordBet.Cost,
                            SettleAmount = recordBet.PlayerPrize,
                            TieBetAdjustment = null,
                            ClientStatus = _service.GetClientRound(recordBet.State),
                            AdditionalData = _jsonSerializationProvider.SerializeToString(recordBet),
                            Success = success
                        });
                        ;
                    }
                }
            }
        }
    }
}
