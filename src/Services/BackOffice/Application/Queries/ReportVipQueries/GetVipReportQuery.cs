using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Vip;
using TGS.Services.BackOffice.Application.Models.ReportVip;
using Tgs.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.ReportVipQueries
{
    public class
        GetVipReportQuery : IGetService<GetVipReportReportRequest,
            GetVipReportReportResponse>
    {
        private readonly IReportClient _reportClient;

        public GetVipReportQuery(IReportClient reportClient)
            => _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));

        public async ValueTask<GetVipReportReportResponse> GetAsync(
            GetVipReportReportRequest request,
            CancellationToken cancellationToken = default)
        {
            var reportRequest = new GetVipReportRequest
            {
                BrandId = request.BrandId,
                Username = request.Username,
                Level = request.Level,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = true
            };

            var response = await _reportClient.GetVipReportAsync(reportRequest);

            if (!response.Valid)
            {
                return new GetVipReportReportResponse
                {
                    Valid = response.Valid,
                    Message = response.Message,
                    TotalCount = response.TotalCount,
                    Items = Array.Empty<VipReportModel>(),
                    Total = new VipReportTotalModel(0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                };
            }

            var pageArray = response.Items?.Select(x => new VipReportModel
            {
                BrandId = x.BrandId,
                Username = x.Username,
                PlayerCode = x.PlayerCode,
                Level = x.Level,
                DepositAmount = x.DepositAmount,
                WithdrawalAmount = x.WithdrawalAmount,
                BonusAmount = x.BonusAmount,
                CompanyAdjustAmount = x.CompanyAdjustAmount,
                RebateAmount = x.RebateAmount,
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

            var total = new VipReportTotalModel(
                response.Items.Sum(x => x.DepositAmount),
                response.Items.Sum(x => x.WithdrawalAmount),
                response.Items.Sum(x => x.CompanyAdjustAmount),
                response.Items.Sum(x => x.RebateAmount),
                response.Items.Sum(x => x.BonusAmount),
                response.Items.Sum(x => x.BetAmount),
                response.Items.Sum(x => x.ValidBetAmount),
                response.Items.Sum(x => x.Turnover),
                response.Items.Sum(x => x.WinLossAmount),
                response.Items.Sum(x => x.NetProfit)
            );

            return new GetVipReportReportResponse
            {
                TotalCount = response.TotalCount,
                Items = pageArray.Items,
                Total = total
            };
        }
    }
}