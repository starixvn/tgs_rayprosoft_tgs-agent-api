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
    GetMarketingFirstDepositDetailQuery : IGetService<GetMarketingFirstDepositDetailReportRequest,
        PagedResponse<MarketingFirstDepositDetailReportModel>>
{
    private readonly IReportClient _reportClient;

    public GetMarketingFirstDepositDetailQuery(IReportClient reportClient)
        => _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));

    public async ValueTask<PagedResponse<MarketingFirstDepositDetailReportModel>> GetAsync(
        GetMarketingFirstDepositDetailReportRequest request,
        CancellationToken cancellationToken = default)
    {
        var reportRequest = new GetMarketingFirstDepositDetailRequest
        {
            BrandId = request.BrandId,
            AgentUsername = request.AgentUsername,
            Date = request.Date,
            Page = request.Page,
            Rows = request.Rows,
            OrderBy = request.OrderBy,
            SortBy = request.SortBy,
            ShowAll = request.ShowAll
        };

        var response = await _reportClient.GetMarketingFirstDepositDetailsAsync(reportRequest);

        return new()
        {
            Valid = response.Valid,
            Message = response.Message,
            TotalCount = response.TotalCount,
            Items = response.Items.Select(x => new MarketingFirstDepositDetailReportModel
            {
                Username = x.Username,
                FirstDepositOn = x.FirstDepositOn,
                FirstDepositAmount = x.FirstDepositAmount,
                TotalDepositAmount = x.TotalDepositAmount,
                TotalWithdrawAmount = x.TotalWithdrawAmount,
                DepositSubtractWithdraw = x.DepositSubtractWithdraw,
                IpAddress = x.IpAddress
            }).ToArray()
        };
    }
}