using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Infrastructures.WebApiServices;
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
using static TGS.Core.Models.SettingConsts;

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
        public async ValueTask<KYGPGameRecordResponses> GameRecordAsync(KYGPGameRecordRequest request)
        {

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts
                .GameProviderConfigurations.KY);
            configuration.MerchantCodes.TryGetValue(request.CurrencyCode, out string agentId);

            if (string.IsNullOrEmpty(agentId))
            {
                return new KYGPGameRecordResponses()
                {
                    Success = false
                };
            }

            var date = DateTimeOffset.UtcNow.AddHours(+8);
            var timestamp = date.ToUnixTimeMilliseconds();

            var data = new KYGameRecordRequest()
            {
                Action = KYUtility.GetAction(GPAction.GameRecord) ?? -1,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
            };

            var dataPath = KYUtility.GetPath(data);
            var param = KYUtility.AesEncrypt(dataPath, configuration.Deskey, true);
            var plainText = $"{agentId}{timestamp}{configuration.Md5key}";
            var key = _hashProvider.Hash(plainText, HashAlgorithmKind.Md5);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            var response = await _client.GetAPIAsync<KYGameRecordResponses>(configuration.ApiRecordUrl, apiKYRequest);

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(GameProviderCode);
            var players = new Dictionary<string, Player>();
            var gameIds = new Dictionary<int, CachedGame>();
            var count = 0;
            var successCount = 0;
            var failCount = 0;
            var lastGameRecorTime = default(DateTimeOffset?);
            if (response.Data.Code == 0)
            {
                var sysDateTimeOffset = DateTimeOffset.UtcNow;

                count = response.Data.Count;
                for (int i = 0; i < response.Data.Count; i++)
                {
                    var gameId = default(int?);
                    var game = default(CachedGame);
                    var player = default(Player);
                    var gameProviderConfigurationId = default(Guid?);
                    var success = true;
                    var PlayerCode = response.Data.List.Accounts[i].Split('_')[1];
                    player = players.Where(x => x.Key == PlayerCode).FirstOrDefault().Value;
                    if (player == null)
                    {
                        player = await _playerOperations.GetPlayerByCodeAsync(PlayerCode);
                        if (player != null)
                        {
                            players.Add(PlayerCode, player);
                        }
                    }
                    if (player != null)
                    {
                        gameProviderConfigurationId = await _brandOperations.GetBrandGameProviderConfigurationIdAsync(player.BrandId, gameProviderId);
                    }


                    game = gameIds.Where(x => x.Key == response.Data.List.KindID[i]).FirstOrDefault().Value;
                    if (game == null)
                    {
                        var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, response.Data.List.KindID[i].ToString()).ConfigureAwait(false);

                        if (games != null && games.Length > 0)
                        {
                            game = games.FirstOrDefault();
                            gameIds.Add(response.Data.List.KindID[i], game);
                        }
                    }


                    if (game != null)
                    {
                        gameId = game.Id;
                    }

                    if (player == null || !gameId.HasValue || !gameProviderConfigurationId.HasValue)
                    {
                        success = false;
                    }


                    var result = false;
                    try
                    {
                        result = await _gameRoundsProcessor.ExecuteAsync(new GameRoundsData()
                        {
                            ExternalRoundId = response.Data.List.GameID[i],
                            GameProviderId = gameProviderId,
                            PlayerCode = player.PlayerCode,
                            GameId = gameId.GetValueOrDefault(-1),
                            CurrencyCode = request.CurrencyCode,
                            Status = RoundStatus.Closed,
                            GameProviderConfigurationId = gameProviderConfigurationId.GetValueOrDefault(),
                            WalletId = player.DefaultWalletId,
                            CreatedOn = sysDateTimeOffset,
                            ClosedOn = sysDateTimeOffset,
                            UpdatedOn = sysDateTimeOffset,
                            BrandId = player.BrandId,
                            ExternalCreatedOn = new DateTimeOffset(DateTime.Parse(response.Data.List.GameStartTime[i]), TimeSpan.FromHours(+8)),
                            ExternalClosedOn = new DateTimeOffset(DateTime.Parse(response.Data.List.GameEndTime[i]), TimeSpan.FromHours(+8)),
                            BetAmount = Convert.ToDecimal(response.Data.List.AllBet[i]),
                            ValidBetAmount = Convert.ToDecimal(response.Data.List.AllBet[i]),
                            WinLoss = Convert.ToDecimal(response.Data.List.Profit[i]).Negate(),
                            Commission = Convert.ToDecimal(response.Data.List.Revenue[i]),
                            Turnover = Convert.ToDecimal(response.Data.List.CellScore[i]),
                            SettleAmount = Convert.ToDecimal(response.Data.List.AllBet[i]) + Convert.ToDecimal(response.Data.List.Profit[i]),
                            TieBetAdjustment = null,
                            ClientStatus = GetClientRound(Convert.ToDecimal(response.Data.List.Profit[i])),
                            AdditionalData = JsonSerializer.Serialize(new
                            {
                                GameID = response.Data.List.GameID[i],
                                Accounts = response.Data.List.Accounts[i],
                                ServerID = response.Data.List.ServerID[i],
                                KindID = response.Data.List.KindID[i],
                                TableID = response.Data.List.TableID[i],
                                ChairID = response.Data.List.ChairID[i],
                                UserCount = response.Data.List.UserCount[i],
                                CellScore = response.Data.List.CellScore[i],
                                AllBet = response.Data.List.AllBet[i],
                                Profit = response.Data.List.Profit[i],
                                Revenue = response.Data.List.Revenue[i],
                                GameStartTime = response.Data.List.GameStartTime[i],
                                GameEndTime = response.Data.List.GameEndTime[i],
                                CardValue = response.Data.List.CardValue[i],
                                ChannelID = response.Data.List.ChannelID[i],
                                LineCode = response.Data.List.LineCode[i],
                            }),
                            Success = success
                        });
                        lastGameRecorTime = new DateTimeOffset(DateTime.Parse(response.Data.List.GameEndTime[i]), TimeSpan.FromHours(+8));
                    }
                    catch (Exception ex)
                    {
                        result = false;
                    }

                    if (result)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                    }
                }
            }


            var responses = new KYGPGameRecordResponses()
            {
                Success = (response.Data.Code == 0 || response.Data.Code == 16) ? true : false,
                LastGameRecorTime = lastGameRecorTime,
                Count = (response.Data.Code == 0 || response.Data.Code == 16) ? response.Data.Count : 0,
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




    public record RoundResult
    {
        //public Guid RoundId { get; set; }
        public string ExternalRoundId { get; init; }
        public Guid GameProviderId { get; init; }
        public string PlayerCode { get; init; }
        public int GameId { get; init; }
        public string CurrencyCode { get; init; }
        public RoundStatus Status { get; init; }
        public Guid GameProviderConfigurationId { get; init; }
        public Guid WalletId { get; init; }
        public DateTimeOffset CreatedOn { get; init; }
        public DateTimeOffset? ClosedOn { get; init; }
        public DateTimeOffset UpdatedOn { get; init; }
        public DateTimeOffset? ExternalCreatedOn { get; init; }
        public DateTimeOffset? ExternalClosedOn { get; init; }
        public Guid BrandId { get; init; }
        public decimal? BetAmount { get; init; }
        public decimal? ValidBetAmount { get; init; }
        public decimal? SettleAmount { get; init; }
        public decimal? Commission { get; init; }
        public decimal? Turnover { get; init; }
        public decimal? WinLoss { get; init; }
        public decimal? TieBetAdjustment { get; init; }
        public virtual GameRoundsDetailData RoundDetail { get; init; }
        public ClientRoundStatus ClientStatus { get; init; }
        public bool Success { get; init; }
    }

    public record GameRoundsDetailData
    {
        public string AdditionalData { get; set; }
    }
}
