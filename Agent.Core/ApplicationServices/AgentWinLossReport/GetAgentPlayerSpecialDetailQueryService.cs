using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Report.Client;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.AgentWinLossReport;

public class
    GetAgentPlayerSpecialDetailQueryService : IGetService<GetAgentPlayerSpecialDetailQuery,
        AgentPlayerSpecialDetailModel>
{
    private readonly IReportClient _reportClient;

    public GetAgentPlayerSpecialDetailQueryService(
        IReportClient reportClient)
    {
        _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));
    }

    public async ValueTask<AgentPlayerSpecialDetailModel> GetAsync(
        GetAgentPlayerSpecialDetailQuery request,
        CancellationToken cancellationToken = default)
    {
        var response = await _reportClient.GetAgentPlayerSpecialDetailAsync(new()
        {
            BrandId = request.BrandId,
            AgentId = request.AgentId,
            From = request.From,
            To = request.To
        }).ConfigureAwait(false);

        return new AgentPlayerSpecialDetailModel
        {
            Bonus = response.Bonus,
            BonusAdjust = response.BonusAdjust,
            PlatformCharge = response.PlatformCharge,
            ProductFee = response.ProductFee
        };
    }
}