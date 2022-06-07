using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.PlayerDeposit;
using TGS.Services.BackOffice.Application.Models.ReportPlayerDeposit;
using Tgs.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.ReportPlayerDepositQueries;

public class
    GetPlayerDepositsQuery : IGetService<GetPlayerDepositsReportRequest,
        PagedResponse<PlayerDepositReportModel>>
{
    private readonly IReportClient _reportClient;

    public GetPlayerDepositsQuery(IReportClient reportClient)
        => _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));

    public async ValueTask<PagedResponse<PlayerDepositReportModel>> GetAsync(
        GetPlayerDepositsReportRequest request,
        CancellationToken cancellationToken = default)
    {
        var reportRequest = new GetPlayerDepositsRequest
        {
            BrandId = request.BrandId,
            AgentUsername = request.AgentUsername,
            From = request.From,
            To = request.To,
            Page = request.Page,
            Rows = request.Rows,
            OrderBy = request.OrderBy,
            SortBy = request.SortBy
        };

        var response = await _reportClient.GetPlayerDepositsAsync(reportRequest);

        return new()
        {
            Valid = response.Valid,
            Message = response.Message,
            TotalCount = response.TotalCount,
            Items = response.Items.Select(x => new PlayerDepositReportModel
            {
                ReportDate = x.ReportDate,
                FirstDepositCount = x.FirstDepositCount,
                NoSecondDepositCount = x.NoSecondDepositCount
            }).ToArray()
        };
    }
}