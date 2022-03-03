using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Agent;
using TGS.Services.BackOffice.Application.Models.ReportAgent;
using Tgs.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.ReportAgentQueries
{
    public class
        GetAgentDividendsQuery : IGetService<GetAgentDividendsReportRequest, PagedResponse<AgentDividendReportModel>>
    {
        private readonly IReportClient _reportClient;

        public GetAgentDividendsQuery(IReportClient reportClient)
            => _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));

        public async ValueTask<PagedResponse<AgentDividendReportModel>> GetAsync(GetAgentDividendsReportRequest request,
            CancellationToken cancellationToken = default)
        {
            var reportRequest = new GetAgentDividendsRequest
            {
                BrandId = request.BrandId,
                AgentId = request.AgentId,
                Username = request.Username,
                Type = (short?)request.Type,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _reportClient.GetAgentDividendsAsync(reportRequest);

            return new()
            {
                Valid = response.Valid,
                Message = response.Message,
                TotalCount = response.TotalCount,
                Items = response.Items?.Select(x => new AgentDividendReportModel
                {
                    Id = x.Id,
                    Username = x.Username,
                    DepositAmount = x.DepositAmount,
                    WithdrawalAmount = x.WithdrawalAmount,
                    AdjustmentAmount = x.AdjustmentAmount,
                    BonusAmount = x.BonusAmount,
                    BetAmount = x.BetAmount,
                    ValidBetAmount = x.ValidBetAmount,
                    Turnover = x.Turnover,
                    WinLossAmount = x.WinLossAmount,
                    NetProfit = x.NetProfit,
                    AgentDividendAmount = x.AgentDividendAmount,
                    FromPlayerAgentDividendAmount = x.FromPlayerAgentDividendAmount,
                    FromTeamAgentDividendAmount = x.FromTeamAgentDividendAmount
                }).ToArray()
            };
        }
    }
}