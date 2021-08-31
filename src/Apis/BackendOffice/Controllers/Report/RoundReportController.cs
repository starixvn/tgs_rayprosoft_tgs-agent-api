using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.Round;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Queries.Game;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoundReportController : BaseContextController
    {
        private readonly IReportClient _client;

        public RoundReportController(IReportClient client)
        {
            _client = client;
        }

        [HttpGet("{roundId}")]
        [Description("Get a round")]
        public Task<GetRoundHistoryResponse> GetRoundAsync(Guid roundId, [FromQuery] GetRoundHistoryRequest request)
        {
            request.RoundId = roundId;
            return _client.GetRoundHistoriesAsync(request);
        }

        [HttpGet]
        [Description("Get rounds")]
        public async ValueTask<PagedResponse<RoundView>> GetRoundsAsync(
            [FromServices] IGetService<GetGameRoundListQuery, PagedResponse<RoundModel>> queryService,
            [FromServices] IQueryService<GetAllGamesByBrandIdQuery, GameData> gameQueryService,
            [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
            [FromServices] IJsonSerializationProvider jsonSerializationProvider,
            [FromQuery] GetRoundsRequest request)
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
                request);

            var records = await queryService
                .GetAsync(query)
                .ConfigureAwait(false);

            var allGames = await gameQueryService
                .QueryAsync(new GetAllGamesByBrandIdQuery(request.BrandId))
                .Select(game => new
                {
                    GameId = game.GameId,
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
                         let gameLocalization = allGames[model.GameId].Localization
                         let loc = gameLocalization.TryGetValue(currentCulture, out var data)
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
                             GameProviderId = model.GameProviderId,
                             PlayerCode = model.PlayerCode,
                             PlatformType = model.PlatformType,
                             PlayerName = model.PlayerName,
                             SettleAmount = model.SettleAmount,
                             Status = model.Status,
                             SubGameProviderName = model.SubGameProviderName,
                             TieBetAdjustment = model.TieBetAdjustment,
                             UpdatedOn = model.UpdatedOn,
                             ValidBetAmount = model.ValidBetAmount,
                             WinLoss = model.WinLoss
                         }).ToArray()
            };
        }
    }
}
