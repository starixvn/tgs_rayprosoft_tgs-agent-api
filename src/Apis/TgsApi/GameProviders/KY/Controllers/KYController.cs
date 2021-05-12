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
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.GameOperations.Data;
using TGS.Services.GameOperations.Models.Data;
using TGS.Services.Operations;
using TGS.Shared.Game.ApiClients.KY;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;
using TGS.Shared.Game.Models.KY.Commons;
using TGS.Shared.Game.Models.KY.Incoming;
using TGS.Shared.Game.Models.KY.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.KY.Controllers
{
    [Route("api/ky")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.KY)]
    public class KYController : ControllerBase
    {
        private static readonly string GameProviderCode = Consts.GameProvider.KY;
        private readonly IKYApiClient _client;
        private readonly ISettingOperations _settingOperations;
        private readonly IHashProvider _hashProvider;
        private readonly IPlayerOperations _playerOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameRoundsProcessor _gameRoundsProcessor;

        public KYController(
            IKYApiClient client,
            ISettingOperations settingOperations,
            IHashProvider hashProvider,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IGameRoundsProcessor gameRoundsProcessor
          )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
        }

        [HttpPost("GameRecord")]
        public async ValueTask<KYGPGameRecordResponse> GameRecordAsync(KYGPGameRecordRequest request)
        {

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts
                .GameProviderConfigurations.KY);

            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(request.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }

            var agentId = brandMerchant.AgentMappers.Where(x => x.CurrencyCode.IgnoreCaseEquals(request.CurrencyCode) && x.Status).FirstOrDefault()?.AgentId;

            if (string.IsNullOrEmpty(agentId))
            {
                return new KYGPGameRecordResponse()
                {
                    Success = false
                };
            }

            var date = DateTimeOffset.UtcNow.AddHours(+8);
            var timestamp = date.ToUnixTimeMilliseconds();

            var data = new KYGameRecordRequest()
            {
                Action = KYUtility.GetAction(GPAction.GameRecord) ?? -1,
                StartTime = request.StartTime.ToUnixTimeMilliseconds(),
                EndTime = request.EndTime.ToUnixTimeMilliseconds(),
            };

            var dataPath = KYUtility.GetPath(data);
            var param = KYUtility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _hashProvider.Hash(plainText, HashAlgorithmKind.Md5);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            var response = await _client.GetAPIAsync<KYGameRecordResponse>(brandMerchant.ApiRecordUrl, apiKYRequest);

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(GameProviderCode);
            var players = new Dictionary<string, Player>();
            var gameIds = new Dictionary<int, CachedGame>();
            var count = 0;
            var successCount = 0;
            var failCount = 0;
            var lastGameRecordTime = default(DateTimeOffset?);
            if (response.Data.Code == 0)
            {
                var sysDateTimeOffset = DateTimeOffset.UtcNow;

                for (int i = 0; i < response.Data.Count; i++)
                {
                    count++;

                    var gameRecord = new KYGameRecord()
                    {
                        GameID = response.Data.List.GameID[i],
                        Account = response.Data.List.Accounts[i],
                        ServerID = response.Data.List.ServerID[i],
                        KindID = response.Data.List.KindID[i],
                        TableID = response.Data.List.TableID[i],
                        ChairID = response.Data.List.ChairID[i],
                        UserCount = response.Data.List.UserCount[i],
                        CellScore = Convert.ToDecimal(response.Data.List.CellScore[i]),
                        AllBet = Convert.ToDecimal(response.Data.List.AllBet[i]),
                        Profit = Convert.ToDecimal(response.Data.List.Profit[i]),
                        Revenue = Convert.ToDecimal(response.Data.List.Revenue[i]),
                        GameStartTime = new DateTimeOffset(DateTime.Parse(response.Data.List.GameStartTime[i]), TimeSpan.FromHours(+8)),
                        GameEndTime = new DateTimeOffset(DateTime.Parse(response.Data.List.GameEndTime[i]), TimeSpan.FromHours(+8)),
                        CardValue = response.Data.List.CardValue[i],
                        ChannelID = Convert.ToInt32(response.Data.List.ChannelID[i]),
                        LineCode = response.Data.List.LineCode[i]
                    };


                    var gameId = default(int?);
                    var gameProviderConfigurationId = default(Guid?);
                    var success = true;
                    var PlayerCode = gameRecord.Account.Split('_')[1];

                    #region Get Player
                    if (players.TryGetValue(PlayerCode, out Player player))
                    {
                        gameProviderConfigurationId = await _brandOperations.GetBrandGameProviderConfigurationIdAsync(player.BrandId, gameProviderId);

                    }
                    else
                    {
                        player = await _playerOperations.GetPlayerByCodeAsync(PlayerCode);
                        if (player != null)
                        {
                            gameProviderConfigurationId = await _brandOperations.GetBrandGameProviderConfigurationIdAsync(player.BrandId, gameProviderId);
                            players.Add(PlayerCode, player);
                        }
                    }
                    #endregion

                    #region Get Game
                    if (gameIds.TryGetValue(gameRecord.KindID, out CachedGame game))
                    {
                        gameId = game.Id;
                    }
                    else
                    {
                        var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, gameRecord.KindID.ToString()).ConfigureAwait(false);

                        if (games != null && games.Length > 0)
                        {
                            game = games[0];
                            gameId = game.Id;
                            gameIds.Add(response.Data.List.KindID[i], game);
                        }
                    }
                    #endregion

                    if (player == null || !gameId.HasValue || !gameProviderConfigurationId.HasValue)
                    {
                        success = false;
                        failCount++;
                    }
                    else
                    {
                        successCount++;
                    }

                    try
                    {
                        var result = await _gameRoundsProcessor.ExecuteAsync(new GameRoundsData()
                        {
                            ExternalRoundId = gameRecord.GameID,
                            GameProviderId = gameProviderId,
                            GameProviderCode = GameProviderCode,
                            PlayerCode = player.PlayerCode,
                            GameId = gameId.GetValueOrDefault(-1),
                            ExternalId = gameRecord.KindID.ToString(),
                            CurrencyCode = request.CurrencyCode,
                            Status = RoundStatus.Closed,
                            GameProviderConfigurationId = gameProviderConfigurationId.GetValueOrDefault(),
                            WalletId = player.DefaultWalletId,
                            CreatedOn = sysDateTimeOffset,
                            ClosedOn = sysDateTimeOffset,
                            UpdatedOn = sysDateTimeOffset,
                            BrandId = player.BrandId,
                            ExternalCreatedOn = gameRecord.GameStartTime,
                            ExternalClosedOn = gameRecord.GameEndTime,
                            BetAmount = gameRecord.AllBet,
                            ValidBetAmount = gameRecord.AllBet,
                            PlayerWinLoss = gameRecord.Profit,
                            Commission = gameRecord.Revenue,
                            Turnover = gameRecord.CellScore,
                            SettleAmount = gameRecord.AllBet + gameRecord.Profit,
                            TieBetAdjustment = null,
                            ClientStatus = GetClientRound(gameRecord.Profit),
                            AdditionalData = JsonSerializer.Serialize(gameRecord),
                            Success = success
                        });
                        lastGameRecordTime = new DateTimeOffset(DateTime.Parse(response.Data.List.GameEndTime[i]), TimeSpan.FromHours(+8));
                    }
                    catch
                    {
                        break;
                    }
                }
            }

            var responses = new KYGPGameRecordResponse()
            {
                Success = (response.Data.Code is 0 or 16),
                LastGameRecorTime = lastGameRecordTime,
                Count = (response.Data.Code is 0 or 16) ? count : 0,
                SuccessCount = successCount,
                FailCount = failCount
            };
            return responses;
        }

        private ClientRoundStatus GetClientRound(decimal playerWinLoss)
        {
            return playerWinLoss switch
            {
                > 0 => ClientRoundStatus.Win,
                < 0 => ClientRoundStatus.Loss,
                0 => ClientRoundStatus.Draw
            };
        }
    }
}
