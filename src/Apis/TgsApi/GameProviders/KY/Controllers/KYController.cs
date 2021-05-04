using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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
using TGS.Services.Operations;
using TGS.Shared.Game.ApiClients.KY;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;
using TGS.Shared.Game.Models.KY.Commons;
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

        public KYController(
            IKYApiClient client,
            ISettingOperations settingOperations,
            IHashProvider hashProvider,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations
          )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
        }

        [HttpPost("GameRecord")]
        public async ValueTask<List<RoundResult>> GameRecordAsync(KYGameRecord request)
        {

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts
                .GameProviderConfigurations.KY);
            configuration.MerchantCodes.TryGetValue(request.CurrencyCode, out string agentId);

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

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.KY);
            var players = new Dictionary<string, Player>();
            var gameIds = new Dictionary<int, CachedGame>();
            var list = default(List<RoundResult>);
            if (response.Data.Code == 0)
            {
                var sysDateTimeOffset = DateTimeOffset.UtcNow;
                list = new List<RoundResult>();
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

                    list.Add(new RoundResult()
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
                        ExternalCreatedOn = DateTimeOffset.Parse(response.Data.List.GameStartTime[i]),
                        ExternalClosedOn = DateTimeOffset.Parse(response.Data.List.GameEndTime[i]),
                        BetAmount = Convert.ToDecimal(response.Data.List.AllBet[i]),
                        ValidBetAmount = Convert.ToDecimal(response.Data.List.AllBet[i]),
                        WinLoss = Convert.ToDecimal(response.Data.List.Profit[i]).Negate(),
                        Commission = Convert.ToDecimal(response.Data.List.Revenue[i]),
                        Turnover = Convert.ToDecimal(response.Data.List.CellScore[i]),
                        SettleAmount = Convert.ToDecimal(response.Data.List.AllBet[i]) + Convert.ToDecimal(response.Data.List.Profit[i]),
                        TieBetAdjustment = null,
                        ClientStatus = GetClientRound(Convert.ToDecimal(response.Data.List.Profit[i])),
                        RoundDetail = new RoundDetailResult() { AdditionalData = response.Data.List.CardValue[i] },
                        Success = success
                    });
                }
            }

            return list;
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

    public record KYGameRecord
    {
        public string CurrencyCode { get; init; }
        public long StartTime { get; init; }
        public long EndTime { get; init; }
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
        public virtual RoundDetailResult RoundDetail { get; init; }
        public ClientRoundStatus ClientStatus { get; init; }
        public bool Success { get; init; }
    }

    public record RoundDetailResult
    {
        public string AdditionalData { get; set; }
    }
}
