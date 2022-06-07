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
    GetMarketingRegisterDetailQuery : IGetService<GetMarketingRegisterDetailReportRequest,
        PagedResponse<MarketingRegisterDetailReportModel>>
{
    private readonly IReportClient _reportClient;

    public GetMarketingRegisterDetailQuery(IReportClient reportClient)
        => _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));

    public async ValueTask<PagedResponse<MarketingRegisterDetailReportModel>> GetAsync(
        GetMarketingRegisterDetailReportRequest request,
        CancellationToken cancellationToken = default)
    {
        var reportRequest = new GetMarketingRegisterDetailRequest
        {
            BrandId = request.BrandId,
            AgentUsername = request.AgentUsername,
            Date = request.Date,
            Page = request.Page,
            Rows = request.Rows,
            OrderBy = request.OrderBy,
            SortBy = request.SortBy
        };

        var response = await _reportClient.GetMarketingRegisterDetailsAsync(reportRequest);

        return new()
        {
            Valid = response.Valid,
            Message = response.Message,
            TotalCount = response.TotalCount,
            Items = response.Items.Select(x => new MarketingRegisterDetailReportModel
            {
                Username = x.Username,
                RegisteredOn = x.RegisteredOn,
                LastLoginOn = x.LastLoginOn,
                LoginCount = x.LoginCount
            }).ToArray()
        };
    }
}