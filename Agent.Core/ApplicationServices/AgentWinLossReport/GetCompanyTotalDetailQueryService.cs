using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Report.Client;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.AgentWinLossReport;

public class
    GetCompanyTotalDetailQueryService(
        IReportClient reportClient,
        IBrandRepository brandService)
    : IGetService<GetCompanyTotalDetailQuery,
        IEnumerable<CompanyTotalDetailModel>>
{
    private readonly IBrandRepository _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
    private readonly IReportClient _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));

    public async ValueTask<IEnumerable<CompanyTotalDetailModel>> GetAsync(
        GetCompanyTotalDetailQuery request,
        CancellationToken cancellationToken = default)
    {
        var brand = await _brandService.GetBrandInfoAsync(request.BrandId).ConfigureAwait(false);

        var response = await _reportClient.GetCompanyTotalDetailAsync(new()
        {
            BrandId = request.BrandId,
            AgentId = request.AgentId,
            PlatformCharge = brand!.PlatformCharge,
            From = request.From,
            To = request.To
        }).ConfigureAwait(false);

        return response.Select(x => new CompanyTotalDetailModel
        {
            From = x.From,
            To = x.To,
            Percentage = x.Percentage,
            PlayerWinLoss = x.PlayerWinLoss,
            PlayerSpecial = x.PlayerSpecial,
            CompanyWinLoss = x.CompanyWinLoss,
            CompanySpecial = x.CompanySpecial,
            CompanyTotal = x.CompanyTotal
        });
    }
}