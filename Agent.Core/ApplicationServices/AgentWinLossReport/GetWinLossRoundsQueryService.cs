using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.PlayerService.Client;
using Tgs.Report.Client;
using Tgs.ServiceUtility;
using TGS.Core.Models.Enums;

namespace TGS.Services.Agent.ApplicationServices.AgentWinLossReport;

public class
    GetWinLossRoundsQueryService : IGetService<GetWinLossRoundsQuery,
        RoundsResponse>
{
    private readonly IReportClient _reportClient;
    private readonly IPlayerServiceClient _playerClient;

    public GetWinLossRoundsQueryService(
        IReportClient reportClient,
        IPlayerServiceClient playerClient)
    {
        _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));
        _playerClient = playerClient ?? throw new ArgumentNullException(nameof(playerClient));
    }

    public async ValueTask<RoundsResponse> GetAsync(
        GetWinLossRoundsQuery request,
        CancellationToken cancellationToken = default)
    {
        var playerProperties =
            await _playerClient.GetPlayerPropertiesAsync(request.PlayerCode,
                cancellationToken);

        if (!(Guid.TryParse(playerProperties["AgentId"], out var agentId) && request.AgentId == agentId))
        {
            return new RoundsResponse();
        }

        var response = await _reportClient.GetRoundsAsync(new()
        {
            BrandId = request.BrandId,
            PlayerCode = request.PlayerCode,
            ClosedFrom = request.ClosedFrom,
            ClosedTo = request.ClosedTo,
            Page = request.Page,
            Rows = request.Rows,
            OrderBy = (short)request.OrderBy,
            SortBy = request.SortBy,
            ShowAll = request.ShowAll
        });

        var responseTotal = await _reportClient.GetRoundsTotalAsync(new()
        {
            BrandId = request.BrandId,
            PlayerCode = request.PlayerCode,
            ClosedFrom = request.ClosedFrom,
            ClosedTo = request.ClosedTo
        });

        return new RoundsResponse
        {
            Valid = response.Valid,
            TotalCount = response.TotalCount,
            Items = response.Items.Select(x => new RoundModel
            {
                Id = x.Id,
                Status = (ClientRoundStatus)x.Status,
                PlayerCode = x.PlayerCode,
                ExternalPlayerCode = x.ExternalPlayerCode,
                PlayerName = x.PlayerName,
                GameProviderId = x.GameProviderId,
                GameProviderCode = x.GameProviderCode,
                GameProviderName = x.GameProviderName,
                SubGameProviderName = x.SubGameProviderName,
                GameId = x.GameId,
                GameName = x.GameName,
                ExternalRoundId = x.ExternalRoundId,
                ExternalCreatedOn = x.ExternalCreatedOn,
                ExternalClosedOn = x.ExternalClosedOn,
                CreatedOn = x.CreatedOn,
                ClosedOn = x.ClosedOn,
                CurrencyCode = x.CurrencyCode,
                BetAmount = x.BetAmount,
                ValidBetAmount = x.ValidBetAmount,
                Turnover = x.Turnover,
                SettleAmount = x.SettleAmount,
                WinLoss = x.WinLoss,
                AdditionalData = x.AdditionalData
            }).ToArray(),
            Total = new RoundTotalModel
            {
                BetAmount = responseTotal.BetAmount ?? 0M,
                ValidBetAmount = responseTotal.ValidBetAmount ?? 0M,
                WinLoss = responseTotal.WinLoss ?? 0M,
                SettleAmount = responseTotal.SettleAmount ?? 0M
            }
        };
    }
}