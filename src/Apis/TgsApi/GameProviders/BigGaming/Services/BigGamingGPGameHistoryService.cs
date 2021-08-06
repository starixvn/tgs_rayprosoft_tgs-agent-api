using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Services.GameOperations.Data;
using TGS.Services.GameOperations.Models.Data;
using TGS.Shared.Game.ApiClients.BigGaming;
using TGS.Shared.Game.Models.BigGaming.Commons;
using TGS.Shared.Game.Models.BigGaming.Incoming;
using TGS.Shared.Game.Models.BigGaming.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Services
{
    public interface IBigGamingGPGameHistoryService
    {
        ValueTask<BigGamingGPGameRecordResponse> GameRecordFishingAsync(BigGamingGPGameRecordRequest source);
    }
    public class BigGamingGPGameHistoryService : IBigGamingGPGameHistoryService
    {
        private readonly IBigGamingApiClient _client;
        private readonly IBigGamingService _service;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IGameRoundsProcessor _gameRoundsProcessor;

        public BigGamingGPGameHistoryService(
            IBigGamingApiClient client,
            IBigGamingService service,
            IJsonSerializationProvider jsonSerializationProvider,
            IGameRoundsProcessor gameRoundsProcessor)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
        }
        async ValueTask<BigGamingGPGameRecordResponse> IBigGamingGPGameHistoryService.GameRecordFishingAsync(BigGamingGPGameRecordRequest source)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(source.CurrencyCode).ConfigureAwait(false); ;
            int pageIndex = 1;
            int pageSize = 500;
            var gameProviderId = await _service.GetGameProviderIdAsync();
            var players = new Dictionary<string, Player>();
            var gameIds = new Dictionary<int, CachedGame>();
            var brandGameProviderConfigurationIds = new Dictionary<Guid, Guid>();
            var count = 0;
            var successCount = 0;
            var failCount = 0;
            var lastGameRecordTime = default(DateTimeOffset?);
            var sysDateTimeOffset = DateTime.UtcNow;
            var request = new BigGamingGameRecordFishingRequest()
            {
                Random = GuidExtensions.NewSequentialGuid().ToString(),
                Sn = brandMerchant.SecretCode,
                StartTime = source.StartTime.UtcDateTime.AddHours(-4).ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = source.EndTime.UtcDateTime.AddHours(-4).ToString("yyyy-MM-dd HH:mm:ss"),
                PageIndex = pageIndex.ToString(),
                PageSize = pageSize
            };
            while (true)
            {
                var response = await _client.GetGameRecordAsync(brandMerchant, request).ConfigureAwait(false);
                if (response.Error != null)
                {
                    return new BigGamingGPGameRecordResponse()
                    {
                        Success = false
                    };
                }

                var gameRecords = _jsonSerializationProvider.DeserializeFromString<BigGamingResultGameRecordResponse<BigGamingGameRecordFishingResponse>>(_jsonSerializationProvider.SerializeToString(response.Result));
                if (gameRecords.Items == null || gameRecords.Items.Length == 0)
                {
                    break;
                }
                pageIndex++;
                request.PageIndex = pageIndex.ToString();
                var gameRecordItems = gameRecords.Items;
                for (int i = 0; i < gameRecordItems.Length; i++)
                {
                    count++;

                    var gameId = default(int?);

                    var success = true;
                    var gameRecord = gameRecordItems[i];
                    #region Get Player
                    if (!players.TryGetValue(gameRecord.LoginId, out Player player))
                    {
                        player = await _service.GetPlayerByCodeAsync(gameRecord.LoginId).ConfigureAwait(false);
                        players.Add(gameRecord.LoginId, player);
                    }

                    if (!brandGameProviderConfigurationIds.TryGetValue(player.BrandId, out Guid gameProviderConfigurationId))
                    {
                        gameProviderConfigurationId = await _service.GetBrandGameProviderConfigurationIdAsync(player.BrandId, gameProviderId).ConfigureAwait(false);
                        brandGameProviderConfigurationIds.Add(player.BrandId, gameProviderConfigurationId);
                    }
                    #endregion

                    #region Get Game
                    if (gameIds.TryGetValue(gameRecord.GameType, out CachedGame game))
                    {
                        gameId = game.Id;
                    }
                    else
                    {
                        game = await _service.GetCachedGameAsync(gameProviderId, gameRecord.GameType.ToString()).ConfigureAwait(false);

                        if (game != null)
                        {
                            gameId = game.Id;
                            gameIds.Add(gameRecord.GameType, game);
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


                    var orderTime = new DateTimeOffset(DateTime.Parse(gameRecord.OrderTime), TimeSpan.FromHours(-4));
                    await _gameRoundsProcessor.ExecuteAsync(new GameRoundsData()
                    {
                        ExternalRoundId = gameRecord.ExternalRoundId,
                        GameProviderId = gameProviderId,
                        GameProviderCode = _service.GetGameProviderCode(),
                        PlayerCode = (player == null) ? string.Empty : player.PlayerCode,
                        GameId = gameId.GetValueOrDefault(-1),
                        ExternalId = gameRecord.ExternalId.ToString(),
                        CurrencyCode = player.DefaultWalletCurrencyCode,
                        Status = RoundStatus.Closed,
                        GameProviderConfigurationId = gameProviderConfigurationId,
                        WalletId = (player == null) ? Guid.Empty : player.DefaultWalletId,
                        CreatedOn = sysDateTimeOffset,
                        ClosedOn = sysDateTimeOffset,
                        UpdatedOn = sysDateTimeOffset,
                        BrandId = (player == null) ? Guid.Empty : player.BrandId,
                        ExternalCreatedOn = orderTime,
                        ExternalClosedOn = orderTime,
                        BetAmount = gameRecord.BetAmount,
                        ValidBetAmount = gameRecord.ValidAmount,
                        PlayerWinLoss = gameRecord.Payout,
                        Commission = gameRecord.Extend,
                        Turnover = gameRecord.ValidAmount,
                        SettleAmount = gameRecord.ValidAmount + gameRecord.Payout,
                        TieBetAdjustment = null,
                        ClientStatus = _service.GetClientRound(gameRecord.Payout),
                        AdditionalData = _jsonSerializationProvider.SerializeToString(gameRecord),
                        Success = success
                    });
                    lastGameRecordTime = orderTime;

                }
            }
            return new BigGamingGPGameRecordResponse()
            {
                Success = true,
                LastGameRecorTime = lastGameRecordTime,
                Count = count,
                SuccessCount = successCount,
                FailCount = failCount
            };

        }
    }
}