using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Market;
using TGS.Services.BackOffice.Application.Models.ReportAgent;
using TGS.Services.BackOffice.Application.Models.ReportMarket;
using Tgs.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.ReportMarketQueries
{
    public class
        GetMarketsQuery : IGetService<GetMarketsReportRequest,
            GetMarketsReportResponse>
    {
        private readonly IReportClient _reportClient;

        public GetMarketsQuery(IReportClient reportClient)
            => _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));

        public async ValueTask<GetMarketsReportResponse> GetAsync(
            GetMarketsReportRequest request,
            CancellationToken cancellationToken = default)
        {
            var reportRequest = new GetMarketsRequest
            {
                BrandId = request.BrandId,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = true
            };

            var response = await _reportClient.GetMarketsAsync(reportRequest);

            var pageArray = response.Items?.Select(x => new MarketModel
            {
                ReportDate = x.ReportDate,
                RegisteredCount = x.RegisteredCount,
                FirstFundInCount = x.FirstFundInCount,
                DepositAmount = x.DepositAmount,
                WithdrawalAmount = x.WithdrawalAmount,
                AdjustmentAmount = x.AdjustmentAmount,
                BonusAmount = x.BonusAmount,
                BetAmount = x.BetAmount,
                ValidBetAmount = x.ValidBetAmount,
                Turnover = x.Turnover,
                WinLossAmount = x.WinLossAmount,
                NetProfit = x.NetProfit,
                AdditionalExpenses = x.AdditionalExpenses
            }).AsQueryable().CreatePagedResponse(new PageableRequest
            {
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = request.OrderBy,
                SortBy = request.SortBy
            });

            var total = new MarketReportTotalModel(
                response.Items.Sum(x => x.RegisteredCount),
                response.Items.Sum(x => x.FirstFundInCount),
                response.Items.Sum(x => x.DepositAmount),
                response.Items.Sum(x => x.WithdrawalAmount),
                response.Items.Sum(x => x.AdjustmentAmount),
                response.Items.Sum(x => x.BonusAmount),
                response.Items.Sum(x => x.BetAmount),
                response.Items.Sum(x => x.ValidBetAmount),
                response.Items.Sum(x => x.Turnover),
                response.Items.Sum(x => x.WinLossAmount),
                response.Items.Sum(x => x.NetProfit),
                response.Items.Sum(x => x.AdditionalExpenses)
            );

            return new GetMarketsReportResponse
            {
                TotalCount = response.TotalCount,
                Items = pageArray.Items,
                Total = total
            };
    }
    }