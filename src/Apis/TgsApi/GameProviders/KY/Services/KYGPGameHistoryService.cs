using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.GameProviderRecord;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Services.GameOperations.Data;
using TGS.Services.GameOperations.Models.Data;
using TGS.Shared.Game.ApiClients.KY;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;
using TGS.Shared.Game.Models.KY.Commons;
using TGS.Shared.Game.Models.KY.Incoming;
using TGS.Shared.Game.Models.KY.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.KY.Services
{
    public interface IKYGPGameHistoryService
    {
        ValueTask<KYGPGameRecordResponse> GameRecordAsync(KYGPGameRecordRequest source);
    }
    public class KYGPGameHistoryService : IKYGPGameHistoryService
    {
        private readonly IKYApiClient _client;
        private readonly IKYService _service;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IGameRoundsProcessor _gameRoundsProcessor;
        private readonly IGameProviderRecordService _gameProviderRecordService;
        private readonly IHashProvider _hashProvider;
        public KYGPGameHistoryService(
              IKYApiClient client,
              IKYService service,
              IJsonSerializationProvider jsonSerializationProvider,
              IGameRoundsProcessor gameRoundsProcessor,
              IGameProviderRecordService gameProviderRecordService,
              IHashProvider hashProvider)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
            _gameProviderRecordService = gameProviderRecordService ?? throw new ArgumentNullException(nameof(gameProviderRecordService));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
        }

        async ValueTask<KYGPGameRecordResponse> IKYGPGameHistoryService.GameRecordAsync(KYGPGameRecordRequest source)
        {
            var configuration = await _service.GetConfigurationAsync().ConfigureAwait(false);
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var brandMerchant = await _service.GetBrandMerchant(source.BrandCode).ConfigureAwait(false);
            var agentId = brandMerchant.AgentMappers.Where(x => x.CurrencyCode.IgnoreCaseEquals(source.CurrencyCode) && x.Status).FirstOrDefault()?.AgentId;

            if (string.IsNullOrEmpty(agentId))
            {
                return new KYGPGameRecordResponse()
                {
                    Success = false
                };
            }

            var dateTimeOffset = DateTimeOffset.UtcNow.AddHours(+8);

            var data = new KYGameRecordRequest()
            {
                StartTime = source.StartTime.ToUnixTimeMilliseconds(),
                EndTime = source.EndTime.ToUnixTimeMilliseconds()
            };

            var response = await _client.GetGameRecordAsync(brandMerchant, agentId, dateTimeOffset, data);

            var players = new Dictionary<string, (Player Player, Guid? GameProviderConfigurationId)>();
            var games = new Dictionary<int, CachedGame>();
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

                    var success = true;
                    var playerCode = gameRecord.Account.Split('_')[1];

                    if (!players.TryGetValue(playerCode, out (Player Player, Guid? GameProviderConfigurationId) playerInfo))
                    {
                        playerInfo = await _gameProviderRecordService.GetPlayerInfoAsync(gameProviderId, playerCode).ConfigureAwait(false);
                        players.Add(playerCode, playerInfo);
                    }


                    if (!games.TryGetValue(gameRecord.KindID, out CachedGame game))
                    {
                        game = await _service.GetCachedGameAsync(gameProviderId, gameRecord.KindID.ToString()).ConfigureAwait(false);
                        games.Add(response.Data.List.KindID[i], game);
                    }

                    if (playerInfo.Player == null || game == null)
                    {
                        success = false;
                        failCount++;
                    }
                    else
                    {
                        successCount++;
                    }

                    var result = await _gameRoundsProcessor.ExecuteAsync(new GameRoundsData()
                    {
                        ExternalRoundId = gameRecord.GameID,
                        GameProviderId = gameProviderId,
                        GameProviderCode = _service.GetGameProviderCode(),
                        PlayerCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.PlayerCode,
                        GameId = (game == null) ? -1 : game.Id,
                        ExternalId = gameRecord.KindID.ToString(),
                        CurrencyCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.DefaultWalletCurrencyCode,
                        Status = RoundStatus.Closed,
                        GameProviderConfigurationId = playerInfo.GameProviderConfigurationId.GetValueOrDefault(),
                        WalletId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.DefaultWalletId,
                        CreatedOn = sysDateTimeOffset,
                        ClosedOn = sysDateTimeOffset,
                        UpdatedOn = sysDateTimeOffset,
                        BrandId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.BrandId,
                        ExternalCreatedOn = gameRecord.GameStartTime,
                        ExternalClosedOn = gameRecord.GameEndTime,
                        BetAmount = gameRecord.AllBet,
                        ValidBetAmount = gameRecord.AllBet,
                        PlayerWinLoss = gameRecord.Profit,
                        Commission = gameRecord.Revenue,
                        Turnover = gameRecord.CellScore,
                        SettleAmount = gameRecord.AllBet + gameRecord.Profit,
                        TieBetAdjustment = null,
                        ClientStatus = _service.GetClientRound(gameRecord.Profit),
                        AdditionalData = _jsonSerializationProvider.SerializeToString(gameRecord),
                        Success = success
                    });
                    lastGameRecordTime = new DateTimeOffset(DateTime.Parse(response.Data.List.GameEndTime[i]), TimeSpan.FromHours(+8));
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
    }
}
