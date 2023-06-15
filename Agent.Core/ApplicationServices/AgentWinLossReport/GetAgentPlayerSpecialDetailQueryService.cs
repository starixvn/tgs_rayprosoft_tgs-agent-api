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
    private readonly IBrandRepository _brandService;
    private readonly IReportClient _reportClient;

    public GetAgentPlayerSpecialDetailQueryService(
        IReportClient reportClient,
        IBrandRepository brandService)
    {
        _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));
        _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
    }

    public async ValueTask<AgentPlayerSpecialDetailModel> GetAsync(
        GetAgentPlayerSpecialDetailQuery request,
        CancellationToken cancellationToken = default)
    {
        var brand = await _brandService.GetBrandInfoAsync(request.BrandId);

        var response = await _reportClient.GetAgentPlayerSpecialDetailAsync(new()
        {
            BrandId = request.BrandId,
            AgentId = request.AgentId,
            PlatformCharge = brand!.PlatformCharge,
            From = request.From,
            To = request.To
        });

        return new AgentPlayerSpecialDetailModel
        {
            Bonus = response.Bonus,
            BonusAdjust = response.BonusAdjust,
            ProductFee = response.ProductFee
        };
    }
}