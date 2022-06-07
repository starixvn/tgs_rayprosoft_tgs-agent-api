using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Marketing;
using TGS.Services.BackOffice.Application.Models.ReportMarketing;
using Tgs.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.ReportMarketingQueries;

public class
    GetMarketingGamingDetailQuery : IGetService<GetMarketingGamingDetailReportRequest,
        PagedResponse<MarketingGamingDetailReportModel>>
{
    private readonly IReportClient _reportClient;

    public GetMarketingGamingDetailQuery(IReportClient reportClient)
        => _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));

    public async ValueTask<PagedResponse<MarketingGamingDetailReportModel>> GetAsync(
        GetMarketingGamingDetailReportRequest request,
        CancellationToken cancellationToken = default)
    {
        var reportRequest = new GetMarketingGamingDetailRequest
        {
            BrandId = request.BrandId,
            AgentUsername = request.AgentUsername,
            Date = request.Date,
            Page = request.Page,
            Rows = request.Rows,
            OrderBy = request.OrderBy,
            SortBy = request.SortBy
        };

        var response = await _reportClient.GetMarketingGamingDetailsAsync(reportRequest);

        return new()
        {
            Valid = response.Valid,
            Message = response.Message,
            TotalCount = response.TotalCount,
            Items = response.Items.Select(x => new MarketingGamingDetailReportModel
            {
                Username = x.Username,
                GameProviderName = x.GameProviderName,
                SubGameProviderName = x.SubGameProviderName,
                WinLossAmount = x.WinLossAmount
            }).ToArray()
        };
    }
}