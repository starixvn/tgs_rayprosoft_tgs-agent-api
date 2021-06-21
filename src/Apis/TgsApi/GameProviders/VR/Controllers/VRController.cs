using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.VR;
using TGS.Core.Models.Services.Caches;
using TGS.Services.GameOperations.Data;
using TGS.Services.GameOperations.Models.Data;
using TGS.Services.Operations;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;
using TGS.Shared.Game.ApiClients.VR;
using TGS.Shared.Game.Models.VR.Incoming;
using TGS.Shared.Game.Models.VR.Outgoing;

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
        private static readonly string _gameProvider = Consts.GameProvider.VR;
        private readonly IBrandOperations _brandOperations;
        private readonly IVRApiClient _client;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameRoundsProcessor _gameRoundsProcessor;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private int _count;
        private int _failCount;
        private Dictionary<int, CachedGame> _gameIds;
        private DateTimeOffset? _lastGameRecordTime;
        private Dictionary<string, Player> _players;
        private int _successCount;

        public VRController(
            IVRApiClient client,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IGameRoundsProcessor gameRoundsProcessor
        )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _gameProviderOperations =
                gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
        }

        [HttpPost("GameRecord")]
        public async ValueTask<VRGPGameRecordResponse> GameRecordAsync(VRGPGameRecordRequest request)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<VRConfiguration>(SettingConsts
                    .GameProviderConfigurations.VR);

            var brandMerchant = configuration.BrandMerchants
                .Where(x => x.BrandCode.IgnoreCaseEquals(request.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants
                    .Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }


            if (brandMerchant == null)
            {
                return new VRGPGameRecordResponse {Success = false};
            }

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(_gameProvider);
            _players = new Dictionary<string, Player>();
            _gameIds = new Dictionary<int, CachedGame>();
            _count = 0;
            _successCount = 0;
            _failCount = 0;
            _lastGameRecordTime = default(DateTimeOffset?);
            var unsettled = await GetGameRoundAsync(request, brandMerchant, gameProviderId, false);
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

            var settled = await GetGameRoundAsync(request, brandMerchant, gameProviderId, true);
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
        ///     Gets the game round asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="brandMerchant">The brand merchant.</param>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <param name="isUpdateTime">if set to <c>true</c> [is update time].</param>
        private async Task<bool> GetGameRoundAsync(VRGPGameRecordRequest request, VRBrandMerchant brandMerchant,
            Guid gameProviderId, bool isUpdateTime)
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

                var response =
                    await _client.GetAPIAsync<VRGameRecordRequest, VRGameRecordResponse>(brandMerchant,
                        GPAction.GameRecord, data);

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
                        var gameId = default(int?);
                        var gameProviderConfigurationId = default(Guid?);
                        var success = true;
                        var PlayerCode = recordBet.PlayerName;

                        if (isUpdateTime)
                        {
                            _lastGameRecordTime = recordBet.UpdateTime;
                        }

                        #region Get Player

                        if (_players.TryGetValue(PlayerCode, out var player))
                        {
                            gameProviderConfigurationId =
                                await _brandOperations.GetBrandGameProviderConfigurationIdAsync(player.BrandId,
                                    gameProviderId);
                        }
                        else
                        {
                            player = await _playerOperations.GetPlayerByCodeAsync(PlayerCode);
                            if (player != null)
                            {
                                gameProviderConfigurationId =
                                    await _brandOperations.GetBrandGameProviderConfigurationIdAsync(player.BrandId,
                                        gameProviderId);
                                _players.Add(PlayerCode, player);
                            }
                        }

                        #endregion

                        #region Get Game

                        if (_gameIds.TryGetValue(recordBet.ChannelId, out var game))
                        {
                            gameId = game.Id;
                        }
                        else
                        {
                            var games = await _gameProviderOperations
                                .GetCachedGamesAsync(gameProviderId, recordBet.ChannelId.ToString())
                                .ConfigureAwait(false);

                            if (games != null && games.Length > 0)
                            {
                                game = games[0];
                                gameId = game.Id;
                                _gameIds.Add(recordBet.ChannelId, game);
                            }
                        }

                        #endregion

                        if (player == null || game == null || !gameProviderConfigurationId.HasValue)
                        {
                            success = false;
                            _failCount++;
                        }
                        else
                        {
                            _successCount++;
                        }


                        try
                        {
                            var result = await _gameRoundsProcessor.ExecuteAsync(new GameRoundsData
                            {
                                ExternalRoundId = recordBet.SerialNumber,
                                GameProviderId = gameProviderId,
                                GameProviderCode = _gameProvider,
                                PlayerCode = player == null ? string.Empty : player.PlayerCode,
                                GameId = gameId.GetValueOrDefault(-1),
                                ExternalId = recordBet.ChannelId.ToString(),
                                CurrencyCode = player == null ? string.Empty : player.DefaultWalletCurrencyCode,
                                Status = GetRoundStatus(recordBet.State),
                                GameProviderConfigurationId = gameProviderConfigurationId.GetValueOrDefault(),
                                WalletId = player == null ? Guid.Empty : player.DefaultWalletId,
                                CreatedOn = sysDateTimeOffset,
                                ClosedOn = sysDateTimeOffset,
                                UpdatedOn = sysDateTimeOffset,
                                BrandId = player == null ? Guid.Empty : player.BrandId,
                                ExternalCreatedOn = recordBet.CreateTime,
                                ExternalClosedOn = recordBet.UpdateTime,
                                BetAmount = recordBet.Cost,
                                ValidBetAmount = recordBet.State == 1 ? 0 : recordBet.Cost,
                                PlayerWinLoss =
                                    recordBet.State == 1
                                        ? 0
                                        :
                                        GetRoundStatus(recordBet.State) == RoundStatus.Closed
                                            ?
                                            recordBet.PlayerPrize - recordBet.Cost
                                            : null,
                                Commission = 0,
                                Turnover = recordBet.State == 1 ? 0 : recordBet.Cost,
                                SettleAmount = recordBet.PlayerPrize,
                                TieBetAdjustment = null,
                                ClientStatus = GetClientRound(recordBet.State),
                                AdditionalData = JsonSerializer.Serialize(recordBet),
                                Success = success
                            });
                            ;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the round status.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private RoundStatus GetRoundStatus(int state) =>
            state switch
            {
                1 => RoundStatus.Closed,
                2 => RoundStatus.Closed,
                3 => RoundStatus.Closed,
                _ => RoundStatus.Open
            };

        /// <summary>
        ///     Gets the client round.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private ClientRoundStatus GetClientRound(int state) =>
            state switch
            {
                1 => ClientRoundStatus.Void,
                2 => ClientRoundStatus.Loss,
                3 => ClientRoundStatus.Win,
                _ => ClientRoundStatus.Open
            };
    }
}