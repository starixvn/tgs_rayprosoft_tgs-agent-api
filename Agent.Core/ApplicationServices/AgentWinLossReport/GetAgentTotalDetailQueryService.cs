using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Report.Client;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.AgentWinLossReport;

public class
    GetAgentTotalDetailQueryService : IGetService<GetAgentTotalDetailQuery,
        IEnumerable<AgentTotalDetailModel>>
{
    private readonly IBrandRepository _brandService;
    private readonly IReportClient _reportClient;

    public GetAgentTotalDetailQueryService(
        IReportClient reportClient,
        IBrandRepository brandService)
    {
        _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));
        _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
    }

    public async ValueTask<IEnumerable<AgentTotalDetailModel>> GetAsync(
        GetAgentTotalDetailQuery request,
        CancellationToken cancellationToken = default)
    {
        var brand = await _brandService.GetBrandInfoAsync(request.BrandId);

        var response = await _reportClient.GetAgentTotalDetailAsync(new()
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