using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Player;
using Tgs.ServiceUtility;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.ReportPlayer;

namespace TGS.Services.BackOffice.Application.Queries.ReportPlayerQueries
{
    public class
        GetPlayerContributionsQuery : IGetService<GetPlayerContributionsReportRequest,
            GetPlayerContributionsReportResponse>
    {
        private readonly IReportClient _reportClient;

        public GetPlayerContributionsQuery(IReportClient reportClient)
            => _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));

        public async ValueTask<GetPlayerContributionsReportResponse> GetAsync(
            GetPlayerContributionsReportRequest request,
            CancellationToken cancellationToken = default)
        {
            var reportRequest = new GetPlayerContributionsRequest
            {
                BrandId = request.BrandId,
                Username = request.Username,
                HasUpperAgent = request.HasUpperAgent,
                Level = request.Level,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _reportClient.GetPlayerContributionsAsync(reportRequest);

            var pageArray = response.Items?.Select(x => new PlayerContributionReportModel
            {
                Username = x.Username,
                PlayerCode = x.PlayerCode,
                Type = (AccountType)x.Type,
                Level = x.Level,
                DepositAmount = x.DepositAmount,
                WithdrawalAmount = x.WithdrawalAmount,
                AdjustmentAmount = x.AdjustmentAmount,
                BonusAmount = x.BonusAmount,
                BetAmount = x.BetAmount,
                ValidBetAmount = x.ValidBetAmount,
                Turnover = x.Turnover,
                WinLossAmount = x.WinLossAmount,
                NetProfit = x.NetProfit
            }).AsQueryable().CreatePagedResponse(new PageableRequest
            {
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = request.OrderBy,
                SortBy = request.SortBy
            });

            var total = new PlayerContributionReportTotalModel(
                response.Items.Sum(x => x.DepositAmount),
                response.Items.Sum(x => x.WithdrawalAmount),
                response.Items.Sum(x => x.AdjustmentAmount),
                response.Items.Sum(x => x.BonusAmount),
                response.Items.Sum(x => x.BetAmount),
                response.Items.Sum(x => x.ValidBetAmount),
                response.Items.Sum(x => x.Turnover),
                response.Items.Sum(x => x.WinLossAmount),
                response.Items.Sum(x => x.NetProfit)
            );

            return new GetPlayerContributionsReportResponse
            {
                TotalCount = response.TotalCount,
                Items = pageArray.Items,
                Total = total
            };
        }
    }
}
