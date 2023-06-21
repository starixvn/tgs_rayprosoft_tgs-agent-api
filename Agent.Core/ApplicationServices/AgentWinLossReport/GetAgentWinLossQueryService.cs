using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using Tgs.Report.Client;
using Tgs.Report.Client.Models;
using Tgs.Report.Client.Models.AgentWinLoss;
using Tgs.ServiceUtility;
using Tgs.PlayerService.Client;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;

namespace TGS.Services.Agent.ApplicationServices.AgentWinLossReport;

public class
    GetAgentWinLossQueryService : IGetService<GetAgentWinLossQuery, GetAgentWinLossResponse>
{
    private readonly IAgentClient _agentClient;
    private readonly IBrandRepository _brandRepository;
    private readonly IPlayerServiceClient _playerClient;
    private readonly IReportClient _reportClient;

    public GetAgentWinLossQueryService(
        IReportClient reportClient,
        IBrandRepository brandRepository,
        IAgentClient agentClient,
        IPlayerServiceClient playerClient)
    {
        _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        _agentClient = agentClient ?? throw new ArgumentNullException(nameof(agentClient));
        _playerClient = playerClient ?? throw new ArgumentNullException(nameof(playerClient));
    }

    public async ValueTask<GetAgentWinLossResponse> GetAsync(GetAgentWinLossQuery request,
        CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository.GetBrandInfoAsync(request.BrandId).ConfigureAwait(false);

        var response =
            string.IsNullOrEmpty(request.Username) switch
            {
                true =>
                    await GetDownLineAgentWinLossByMasterAgentIdAsync(request, brand!.PlatformCharge).ConfigureAwait(false),
                false when request is { DownLine: false, MemberType: AccountType.Agent } =>
                    await GetAgentWinLossAsync(request, brand!.PlatformCharge, cancellationToken).ConfigureAwait(false),
                false when request is { DownLine: true, MemberType: AccountType.Agent } =>
                    await GetDownLineAgentWinLossAsync(request, brand!.PlatformCharge, cancellationToken).ConfigureAwait(false),
                false when request.MemberType == AccountType.Player =>
                    await GetPlayerWinLossAsync(request, brand!.PlatformCharge, cancellationToken).ConfigureAwait(false),
                _ => null
            };

        if (response == null)
        {
            return new()
            {
                Valid = true,
                Items = Array.Empty<AgentWinLossModel>(),
            };
        }

        var pageArray = response.Items.Select(x
            => new Services.Agent.AgentWinLossModel
            {
                Id = x.Id,
                Username = x.Username,
                MemberType = x.MemberType,
                RoundCount = x.RoundCount,
                Turnover = x.Turnover,
                WinLoss = x.WinLoss,
                AgentWinLoss = x.AgentWinLoss,
                CompanyWinLoss = x.CompanyWinLoss,
                Special = x.Special,
                AgentSpecial = x.AgentSpecial,
                CompanySpecial = x.CompanySpecial,
                AgentTotal = x.AgentTotal,
                CompanyTotal = x.CompanyTotal
            });

        var total = new AgentWinLossTotalModel
        {
            RoundCount = response.Items.Sum(x => x.RoundCount),
            Turnover = response.Items.Sum(x => x.Turnover),
            WinLoss = response.Items.Sum(x => x.WinLoss),
            AgentWinLoss = response.Items.Sum(x => x.AgentWinLoss),
            CompanyWinLoss = response.Items.Sum(x => x.CompanyWinLoss),
            Special = response.Items.Sum(x => x.Special),
            AgentSpecial = response.Items.Sum(x => x.AgentSpecial),
            CompanySpecial = response.Items.Sum(x => x.CompanySpecial),
            AgentTotal = response.Items.Sum(x => x.AgentTotal),
            CompanyTotal = response.Items.Sum(x => x.CompanyTotal)
        };

        return new()
        {
            Valid = true,
            TotalCount = response.TotalCount,
            Items = pageArray.ToArray(),
            Total = total
        };
    }

    private async Task<PagedResponse<Tgs.Report.Client.Models.AgentWinLoss.AgentWinLossModel>> GetAgentWinLossAsync(
        GetAgentWinLossQuery request,
        decimal platformCharge,
        CancellationToken cancellationToken)
    {
        var childAgents =
            _agentClient.GetChildAgentInfosAsync(new GetChildAgentInfosRequest(request.AgentId), cancellationToken);

        var agent = await _agentClient
            .GetAgentInfoByUsernameAsync(new GetAgentInfoByUsernameRequest(request.BrandId, request.Username),
                cancellationToken).ConfigureAwait(false);

        var subAgent =
            await childAgents.FirstOrDefaultAsync(x => x.AgentId == agent.Id, cancellationToken)
                .ConfigureAwait(false);

        var agentId = request.AgentId == agent?.Id ? request.AgentId : subAgent?.AgentId;

        return agentId == null
            ? new PagedResponse<Tgs.Report.Client.Models.AgentWinLoss.AgentWinLossModel>()
            : await _reportClient.GetAgentWinLossAsync(new()
        {
            BrandId = request.BrandId,
            AgentId = agentId.Value,
            PlatformCharge = platformCharge,
            From = request.From,
            To = request.To,
            Page = request.Page,
            Rows = request.Rows,
            OrderBy = (short)request.OrderBy,
            SortBy = request.SortBy,
            ShowAll = request.ShowAll
        }).ConfigureAwait(false);
    }

    private async Task<PagedResponse<Tgs.Report.Client.Models.AgentWinLoss.AgentWinLossModel>>
        GetDownLineAgentWinLossByMasterAgentIdAsync(
            GetAgentWinLossQuery request,
            decimal platformCharge)
    {
        return await _reportClient.GetDownLineAgentWinLossAsync(new()
        {
            BrandId = request.BrandId,
            AgentId = request.AgentId,
            PlatformCharge = platformCharge,
            From = request.From,
            To = request.To,
            Page = request.Page,
            Rows = request.Rows,
            OrderBy = (short)request.OrderBy,
            SortBy = request.SortBy,
            ShowAll = request.ShowAll
        }).ConfigureAwait(false);
    }

    private async Task<PagedResponse<Tgs.Report.Client.Models.AgentWinLoss.AgentWinLossModel>>
        GetDownLineAgentWinLossAsync(
            GetAgentWinLossQuery request,
            decimal platformCharge,
            CancellationToken cancellationToken)
    {
        var childAgents =
            _agentClient.GetChildAgentInfosAsync(new GetChildAgentInfosRequest(request.AgentId), cancellationToken);

        var agent = await _agentClient
            .GetAgentInfoByUsernameAsync(new GetAgentInfoByUsernameRequest(request.BrandId, request.Username),
                cancellationToken).ConfigureAwait(false);

        var subAgent =
            await childAgents.FirstOrDefaultAsync(x => x.AgentId == agent.Id, cancellationToken)
                .ConfigureAwait(false);

        var agentId = request.AgentId == agent?.Id ? request.AgentId : subAgent?.AgentId;

        return agentId == null
            ? new PagedResponse<Tgs.Report.Client.Models.AgentWinLoss.AgentWinLossModel>()
            : await _reportClient.GetDownLineAgentWinLossAsync(new()
        {
            BrandId = request.BrandId,
            AgentId = agentId.Value,
            PlatformCharge = platformCharge,
            From = request.From,
            To = request.To,
            Page = request.Page,
            Rows = request.Rows,
            OrderBy = (short)request.OrderBy,
            SortBy = request.SortBy,
            ShowAll = request.ShowAll
        }).ConfigureAwait(false);
    }

    private async Task<PagedResponse<Tgs.Report.Client.Models.AgentWinLoss.AgentWinLossModel>> GetPlayerWinLossAsync(
        GetAgentWinLossQuery request,
        decimal platformCharge,
        CancellationToken cancellationToken)
    {
        var childAgents =
            _agentClient.GetChildAgentInfosAsync(new GetChildAgentInfosRequest(request.AgentId), cancellationToken);

        var player =
            await _playerClient.GetPlayerInfoByUserNameAsync(request.BrandId,
                request.Username,
                cancellationToken).ConfigureAwait(false);

        if (player == null)
        {
            return new PagedResponse<Tgs.Report.Client.Models.AgentWinLoss.AgentWinLossModel>();
        }

        var playerProperties =
            await _playerClient.GetPlayerPropertiesAsync(player.PlayerCode,
                cancellationToken).ConfigureAwait(false);

        return Guid.TryParse(playerProperties["AgentId"], out var agentId) &&
            await childAgents.AnyAsync(x => x.AgentId == agentId, cancellationToken).ConfigureAwait(false)
            ? await _reportClient.GetAgentPlayerWinLossAsync(new GetPlayerWinLossRequest
            {
                PlayerCode = player.PlayerCode,
                PlatformCharge = platformCharge,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            }).ConfigureAwait(false)
            : new PagedResponse<Tgs.Report.Client.Models.AgentWinLoss.AgentWinLossModel>();
    }
}