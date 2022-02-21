using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Models.Round;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Round;
using TGS.Services.BackOffice.Application.Queries.Game;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;
using Tgs.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoundReportController : BaseContextController
    {
        private readonly IReportClient _client;

        public RoundReportController(IReportClient client) => _client = client;

        [HttpGet("closedRounds")]
        [Description("Get closed rounds")]
        public async Task<PagedResponse<ClosedRoundReportModel>> GetClosedRoundsAsync(
            [FromQuery] GetClosedRoundsReportRequest request)
        {
            var reportRequest = new GetClosedRoundsRequest
            {
                ExternalRoundId = request.ExternalRoundId,
                GameProviderId = request.GameProviderId,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetClosedRoundsAsync(reportRequest);

            return new()
            {
                Valid = response.Valid,
                Message = response.Message,
                TotalCount = response.TotalCount,
                Items = response.Items?.Select(x => new ClosedRoundReportModel
                {
                    AdditionalData = x.AdditionalData,
                    BetAmount = x.BetAmount,
                    BrandCode = x.BrandCode,
                    BrandName = x.BrandName,
                    ClosedOn = x.ClosedOn,
                    Commission = x.Commission,
                    CreatedOn = x.CreatedOn,
                    CurrencyCode = x.CurrencyCode,
                    ExternalClosedOn = x.ExternalClosedOn,
                    ExternalCreatedOn = x.ExternalCreatedOn,
                    ExternalPlayerCode = x.ExternalPlayerCode,
                    ExternalRoundId = x.ExternalRoundId,
                    GameId = x.GameId,
                    GameName = x.GameName,
                    GameProviderCode = x.GameProviderCode,
                    GameProviderId = x.GameProviderId,
                    Id = x.Id,
                    PlatformType = (PlatformType)x.PlatformType,
                    PlayerCode = x.PlayerCode,
                    PlayerName = x.PlayerName,
                    PlayerType = (PlayerType)x.PlayerType,
                    SettleAmount = x.SettleAmount,
                    Status = (ClientRoundStatus)x.Status,
                    SubGameProviderName = x.SubGameProviderName,
                    TieBetAdjustment = x.TieBetAdjustment,
                    UpdatedOn = x.UpdatedOn,
                    ValidBetAmount = x.ValidBetAmount,
                    WinLoss = x.WinLoss
                }).ToArray()
            };
        }

        [HttpGet("openRounds")]
        [Description("Get open rounds")]
        public async Task<PagedResponse<OpenRoundReportModel>> GetOpenRoundsAsync(
            [FromQuery] GetOpenRoundsReportRequest request)
        {
            var reportRequest = new GetOpenRoundsRequest
            {
                ExternalRoundId = request.ExternalRoundId,
                GameProviderId = request.GameProviderId,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetOpenRoundsAsync(reportRequest);

            return new()
            {
                Valid = response.Valid,
                Message = response.Message,
                TotalCount = response.TotalCount,
                Items = response.Items?.Select(x => new OpenRoundReportModel
                {
                    Id = x.Id,
                    AdditionalData = x.AdditionalData,
                    BetAmount = x.BetAmount,
                    BrandCode = x.BrandCode,
                    BrandName = x.BrandName,
                    ClosedOn = x.ClosedOn,
                    Commission = x.Commission,
                    CreatedOn = x.CreatedOn,
                    CurrencyCode = x.CurrencyCode,
                    ExternalClosedOn = x.ExternalClosedOn,
                    ExternalCreatedOn = x.ExternalCreatedOn,
                    ExternalPlayerCode = x.ExternalPlayerCode,
                    ExternalRoundId = x.ExternalRoundId,
                    GameId = x.GameId,
                    GameName = x.GameName,
                    GameProviderCode = x.GameProviderCode,
                    GameProviderId = x.GameProviderId,
                    PlatformType = (PlatformType)x.PlatformType,
                    PlayerCode = x.PlayerCode,
                    PlayerName = x.PlayerName,
                    PlayerType = (PlayerType)x.PlayerType,
                    SettleAmount = x.SettleAmount,
                    Status = (ClientRoundStatus)x.Status,
                    SubGameProviderName = x.SubGameProviderName,
                    TieBetAdjustment = x.TieBetAdjustment,
                    UpdatedOn = x.UpdatedOn,
                    ValidBetAmount = x.ValidBetAmount,
                    WinLoss = x.WinLoss
                }).ToArray()
            };
        }

        [HttpGet("{roundId}")]
        [Description("Get a round")]
        public async Task<JsonDocument?> GetRoundAsync(Guid roundId,
            [FromQuery] GetRoundHistoryReportRequest request)
        {
            var reportRequest = new GetRoundHistoryRequest
            {
                GameProviderCode = request.GameProviderCode,
                PlayerCode = request.PlayerCode,
                RoundId = roundId,
                Language = request.Language
            };

            return await _client.GetRoundHistoriesAsync(reportRequest);
        }

        [HttpGet]
        [Description("Get rounds")]
        public async ValueTask<PagedResponse<RoundView>> GetRoundsAsync(
            [FromServices]
            IGetService<GetGameRoundListQuery, Tgs.Report.Client.Models.PagedResponse<RoundModel>> queryService,
            [FromServices] IQueryService<GetAllGamesByBrandIdQuery, GameData> gameQueryService,
            [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
            [FromServices] IJsonSerializationProvider jsonSerializationProvider,
            [FromQuery] GetRoundsReportRequest request)
        {
            var serializerOptions = jsonOptionsAccessor.Value.JsonSerializerOptions;
            var currentCulture = CultureInfo.CurrentUICulture.Name;

            var query = new GetGameRoundListQuery(
                request.BrandId,
                request.Username,
                request.PlayerCode,
                request.RoundId,
                request.ExternalRoundId,
                request.GameProviderId,
                request.Status,
                request.GameType,
                request.From,
                request.To,
                request.ClosedFrom,
                request.ClosedTo,
                new PageableRequest
                {
                    Page = request.Page,
                    Rows = request.Rows,
                    OrderBy = request.OrderBy,
                    SortBy = request.SortBy,
                    ShowAll = request.ShowAll
                });

            var records = await queryService
                .GetAsync(query)
                .ConfigureAwait(false);

            var allGames = await gameQueryService
                .QueryAsync(new GetAllGamesByBrandIdQuery(request.BrandId))
                .Select(game => new
                {
                    game.GameId,
                    Localization = jsonSerializationProvider
                        .DeserializeFromString<IDictionary<string, GameLocalizationData>>(
                            game.Localization ?? "{}",
                            serializerOptions)
                })
                .ToDictionaryAsync(game => game.GameId)
                .ConfigureAwait(false);

            return new PagedResponse<RoundView>
            {
                Valid = records.Valid,
                Errors = records.Errors,
                Message = records.Message,
                TotalCount = records.TotalCount,
                Items = (from model in records.Items
                         let gameLocalization = allGames.TryGetValue(model.GameId, out var value)
                             ? value.Localization
                             : default
                         let loc = gameLocalization is not null &&
                             gameLocalization.TryGetValue(currentCulture, out var data)
                                 ? data
                                 : default(GameLocalizationData)
                         select new RoundView
                         {
                             Id = model.Id,
                             AdditionalData = model.AdditionalData,
                             BetAmount = model.BetAmount,
                             ClosedOn = model.ClosedOn,
                             Commission = model.Commission,
                             CreatedOn = model.CreatedOn,
                             CurrencyCode = model.CurrencyCode,
                             ExternalClosedOn = model.ExternalClosedOn,
                             ExternalCreatedOn = model.ExternalCreatedOn,
                             ExternalPlayerCode = model.ExternalPlayerCode,
                             ExternalRoundId = model.ExternalRoundId,
                             GameId = model.GameId,
                             GameName = loc?.Name ?? model.GameName,
                             GameProviderCode = model.GameProviderCode,
                             GameProviderName = model.GameProviderName,
                             GameProviderId = model.GameProviderId,
                             PlayerCode = model.PlayerCode,
                             PlatformType = (PlatformType)model.PlatformType,
                             PlayerName = model.PlayerName,
                             SettleAmount = model.SettleAmount,
                             Status = (ClientRoundStatus)model.Status,
                             SubGameProviderName = model.SubGameProviderName,
                             TieBetAdjustment = model.TieBetAdjustment,
                             UpdatedOn = model.UpdatedOn,
                             ValidBetAmount = model.ValidBetAmount,
                             Turnover = model.Turnover,
                             WinLoss = model.WinLoss
                         }).ToArray()
            };
        }
    }
}