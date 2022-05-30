using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Agent;
using Tgs.ServiceUtility;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.ReportAgent;

namespace TGS.Services.BackOffice.Application.Queries.ReportAgentQueries
{
    public class
        GetAgentDividendsQuery : IGetService<GetAgentDividendsReportRequest, GetAgentDividendsReportResponse>
    {
        private readonly IReportClient _reportClient;

        public GetAgentDividendsQuery(IReportClient reportClient)
            => _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));

        public async ValueTask<GetAgentDividendsReportResponse> GetAsync(GetAgentDividendsReportRequest request,
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
                ShowAll = true
            };

            var response = await _reportClient.GetAgentDividendsAsync(reportRequest);

            if (!response.Valid)
            {
                return new GetAgentDividendsReportResponse
                {
                    Valid = response.Valid,
                    Message = response.Message,
                    TotalCount = response.TotalCount,
                    Items = Array.Empty<AgentDividendReportModel>(),
                    Total = new AgentDividendReportTotalModel(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                };
            }

            var pageArray = response.Items?.Select(x => new AgentDividendReportModel
            {
                Id = x.Id,
                Username = x.Username,
                DepositAmount = x.DepositAmount,
                WithdrawalAmount = x.WithdrawalAmount,
                CompanyAdjustAmount = x.CompanyAdjustAmount,
                BonusAdjustAmount = x.BonusAdjustAmount,
                AgentAdjustAmount = x.AgentAdjustAmount,
                BonusAmount = x.BonusAmount,
                BetAmount = x.BetAmount,
                ValidBetAmount = x.ValidBetAmount,
                Turnover = x.Turnover,
                WinLossAmount = x.WinLossAmount,
                NetProfit = x.NetProfit,
                AgentDividendAmount = x.AgentDividendAmount,
                FromPlayerAgentDividendAmount = x.FromPlayerAgentDividendAmount,
                FromTeamAgentDividendAmount = x.FromTeamAgentDividendAmount
            }).AsQueryable().CreatePagedResponse(new PageableRequest
            {
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = request.OrderBy,
                SortBy = request.SortBy
            });

            var total = new AgentDividendReportTotalModel(
                response.Items.Sum(x => x.DepositAmount),
                response.Items.Sum(x => x.WithdrawalAmount),
                response.Items.Sum(x => x.CompanyAdjustAmount),
                response.Items.Sum(x => x.BonusAdjustAmount),
                response.Items.Sum(x => x.AgentAdjustAmount),
                response.Items.Sum(x => x.BonusAmount),
                response.Items.Sum(x => x.BetAmount),
                response.Items.Sum(x => x.ValidBetAmount),
                response.Items.Sum(x => x.Turnover),
                response.Items.Sum(x => x.WinLossAmount),
                response.Items.Sum(x => x.NetProfit),
                response.Items.Sum(x => x.AgentDividendAmount),
                response.Items.Sum(x => x.FromPlayerAgentDividendAmount),
                response.Items.Sum(x => x.FromTeamAgentDividendAmount)
            );

            return new GetAgentDividendsReportResponse
            {
                TotalCount = response.TotalCount,
                Items = pageArray.Items,
                Total = total
            };
        }
    }
}
