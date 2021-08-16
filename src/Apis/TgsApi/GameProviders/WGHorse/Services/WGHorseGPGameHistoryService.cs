using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.TgsApi.Commons.Services.GameProviderRecord;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.GameOperations.Data;
using TGS.Services.GameOperations.Models.Data;
using TGS.Shared.Game.ApiClients.WGHorse;
using TGS.Shared.Game.Models.VR.Outgoing;
using TGS.Shared.Game.Models.WGHorse.Incoming;
using TGS.Shared.Game.Models.WGHorse.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.WGHorse.Services
{
    public interface IWGHorseGPGameHistoryService
    {
        ValueTask<WGHorseGPGameRecordResponse> GameRecordAsync(WGHorseGPGameRecordRequest source);
    }
    public class WGHorseGPGameHistoryService : IWGHorseGPGameHistoryService
    {
        private readonly IWGHorseApiClient _client;
        private readonly IWGHorseService _service;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IGameRoundsProcessor _gameRoundsProcessor;
        private readonly IGameProviderRecordService _gameProviderRecordService;
        private WGHorseConfiguration _configuration;
        private Guid _gameProviderId;
        private Dictionary<string, (Player Player, Guid? GameProviderConfigurationId)> _players;
        private Dictionary<string, CachedGame> _gameIds;
        private Dictionary<Guid, Guid> _brandGameProviderConfigurationIds;

        public WGHorseGPGameHistoryService(
          IWGHorseApiClient client,
          IWGHorseService service,
          IJsonSerializationProvider jsonSerializationProvider,
          IGameRoundsProcessor gameRoundsProcessor,
          IGameProviderRecordService gameProviderRecordService
        )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
            _gameProviderRecordService = gameProviderRecordService ?? throw new ArgumentNullException(nameof(gameProviderRecordService));
        }

        [HttpPost("GameRecord")]
        public async ValueTask<WGHorseGPGameRecordResponse> GameRecordAsync(WGHorseGPGameRecordRequest request)
        {
            _configuration = await _service.GetConfigurationAsync().ConfigureAwait(false);
            _gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            _players = new Dictionary<string, (Player Player, Guid? GameProviderConfigurationId)>();
            _gameIds = new Dictionary<string, CachedGame>();
            _brandGameProviderConfigurationIds = new Dictionary<Guid, Guid>();
            var game = await _service.GetCachedGameAsync(_gameProviderId, "1").ConfigureAwait(false);
            var count = 0;
            var successCount = 0;
            var failCount = 0;
            var lastGameRecordTime = default(DateTimeOffset?);
            var page = 1;
            var rows = 1000;
            var success = true;
            while (true)
            {
                var data = new WGHorseGameRecordRequest
                {
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    PageIndex = page,
                    PageSize = rows
                };

                var response = await _client.GetGameRecordAsync(_configuration, data).ConfigureAwait(false);

                if (response.ReturnCode.IgnoreCaseNotEquals("1"))
                {
                    if (response.ReturnCode.IgnoreCaseNotEquals("503"))
                    {
                        break;
                    }
                    else
                    {
                        throw new GameProviderApiErrorException(response.ReturnCode.ToString());
                    }
                }

                if (page.ToString().IgnoreCaseNotEquals(response.Page))
                {
                    break;
                }

                if (response.ArrayInvoice == null || response.ArrayInvoice.Length == 0)
                {
                    break;
                }



                page++;

                var sysDateTimeOffset = DateTimeOffset.UtcNow;
                var records = response.ArrayInvoice;

                foreach (var record in records)
                {
                    count++;
                    var gameRecord = record;

                    if (!_players.TryGetValue(gameRecord.MemberCode, out (Player Player, Guid? GameProviderConfigurationId) playerInfo))
                    {
                        playerInfo = await _gameProviderRecordService.GetPlayerInfoAsync(_gameProviderId, gameRecord.MemberCode).ConfigureAwait(false);
                        _players.Add(gameRecord.MemberCode, playerInfo);
                    }


                    if (playerInfo.Player == null)
                    {
                        success = false;
                        failCount++;
                    }
                    else
                    {
                        successCount++;
                    }

                    var createdOn = new DateTimeOffset(DateTime.Parse(gameRecord.RecordDate), TimeSpan.FromHours(+8));
                    var externalClosedOn = new DateTimeOffset(DateTime.Parse(gameRecord.ShowDivideDate), TimeSpan.FromHours(+8));
                    await _gameRoundsProcessor.ExecuteAsync(new GameRoundsData()
                    {
                        ExternalRoundId = gameRecord.BatchNo,
                        GameProviderId = _gameProviderId,
                        GameProviderCode = _service.GetGameProviderCode(),
                        PlayerCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.PlayerCode,
                        GameId = (game == null) ? -1 : game.Id,
                        ExternalId = gameRecord.BatchNo.ToString(),
                        CurrencyCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.DefaultWalletCurrencyCode,
                        Status = Core.Models.Enums.RoundStatus.Closed,
                        GameProviderConfigurationId = playerInfo.GameProviderConfigurationId.GetValueOrDefault(),
                        WalletId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.DefaultWalletId,
                        CreatedOn = sysDateTimeOffset,
                        ClosedOn = sysDateTimeOffset,
                        UpdatedOn = sysDateTimeOffset,
                        BrandId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.BrandId,
                        ExternalCreatedOn = createdOn,
                        ExternalClosedOn = externalClosedOn,
                        BetAmount = decimal.Parse(gameRecord.ShowAmount),
                        ValidBetAmount = decimal.Parse(gameRecord.ShowAmount),
                        PlayerWinLoss = decimal.Parse(gameRecord.ShowWin),
                        Commission = 0,
                        Turnover = decimal.Parse(gameRecord.ShowAmount),
                        SettleAmount = decimal.Parse(gameRecord.ShowAmount) + decimal.Parse(gameRecord.ShowWin),
                        TieBetAdjustment = null,
                        ClientStatus = _service.GetClientRound(decimal.Parse(gameRecord.ShowWin)),
                        AdditionalData = _jsonSerializationProvider.SerializeToString(gameRecord),
                        Success = success
                    });
                    lastGameRecordTime = externalClosedOn;


                }
            }
            return new WGHorseGPGameRecordResponse()
            {
                Success = true,
                Count = count,
                SuccessCount = successCount,
                FailCount = failCount,
                LastGameRecorTime = lastGameRecordTime
            };
        }
    }
}