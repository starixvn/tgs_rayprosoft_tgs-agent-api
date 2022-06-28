using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Marketing;
using Tgs.ServiceUtility;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.ReportMarketing;

namespace TGS.Services.BackOffice.Application.Queries.ReportMarketingQueries;

public class
    GetMarketingListQuery : IGetService<GetMarketingListReportRequest,
        PagedResponse<MarketingReportModel>>
{
    private readonly IReportClient _reportClient;

    public GetMarketingListQuery(IReportClient reportClient)
        => _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));

    public async ValueTask<PagedResponse<MarketingReportModel>> GetAsync(
        GetMarketingListReportRequest request,
        CancellationToken cancellationToken = default)
    {
        var reportRequest = new GetMarketingListRequest
        {
            BrandId = request.BrandId,
            AgentUsername = request.AgentUsername,
            From = request.From,
            To = request.To,
            Page = request.Page,
            Rows = request.Rows,
            OrderBy = (short)request.OrderBy,
            SortBy = request.SortBy,
            ShowAll = request.ShowAll
        };

        var response = await _reportClient.GetMarketingListAsync(reportRequest);

        return new PagedResponse<MarketingReportModel>
        {
            Valid = response.Valid,
            Message = response.Message,
            TotalCount = response.TotalCount,
            Items = response.Items.Select(x => new MarketingReportModel
            {
                ReportDate = x.ReportDate,
                FirstDepositCount = x.FirstDepositCount,
                RegisterCount = x.RegisterCount,
                GamingCount = x.GamingCount,
            }).ToArray()
        };
    }
}
