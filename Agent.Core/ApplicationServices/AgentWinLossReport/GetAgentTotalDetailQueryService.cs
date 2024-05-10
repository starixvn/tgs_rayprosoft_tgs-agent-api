using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.AgentWinLoss;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.AgentWinLossReport;

public class
    GetAgentTotalDetailQueryService(
        IReportClient reportClient,
        IBrandRepository brandService)
    : IGetService<GetAgentTotalDetailQuery,
        IEnumerable<AgentTotalDetailModel>>
{
    private readonly IBrandRepository _brandService =
        brandService ?? throw new ArgumentNullException(nameof(brandService));

    private readonly IReportClient
        _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));

    public async ValueTask<IEnumerable<AgentTotalDetailModel>> GetAsync(
        GetAgentTotalDetailQuery request,
        CancellationToken cancellationToken = default)
    {
        var brand = await _brandService.GetBrandInfoAsync(request.BrandId);

        var response = await _reportClient.GetAgentTotalDetailAsync(new GetAgentTotalDetailRequest
        {
            BrandId = request.BrandId,
            AgentId = request.AgentId,
            PlatformCharge = brand!.PlatformCharge,
            From = request.From,
            To = request.To
        }).ConfigureAwait(false);

        return response.Select(x => new AgentTotalDetailModel
        {
            From = x.From,
            To = x.To,
            Percentage = x.Percentage,
            PlayerWinLoss = x.PlayerWinLoss,
            PlayerSpecial = x.PlayerSpecial,
            AgentWinLoss = x.AgentWinLoss,
            AgentSpecial = x.AgentSpecial,
            AgentTotal = x.AgentTotal
        });
    }
}