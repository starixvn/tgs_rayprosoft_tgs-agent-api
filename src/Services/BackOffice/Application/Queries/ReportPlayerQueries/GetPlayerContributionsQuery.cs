using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Player;
using TGS.Services.BackOffice.Application.Models.ReportPlayer;
using Tgs.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.ReportPlayerQueries
{
    public class
        GetPlayerContributionsQuery : IGetService<GetPlayerContributionsReportRequest,
            PagedResponse<PlayerContributionReportModel>>
    {
        private readonly IReportClient _reportClient;

        public GetPlayerContributionsQuery(IReportClient reportClient)
            => _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));

        public async ValueTask<PagedResponse<PlayerContributionReportModel>> GetAsync(
            GetPlayerContributionsReportRequest request,
            CancellationToken cancellationToken = default)
        {
            var reportRequest = new GetPlayerContributionsRequest
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
                ShowAll = request.ShowAll
            };

            var response = await _reportClient.GetPlayerContributionsAsync(reportRequest);

            return new()
            {
                Valid = response.Valid,
                Message = response.Message,
                TotalCount = response.TotalCount,
                Items = response.Items?.Select(x => new PlayerContributionReportModel
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
                }).ToArray()
            };
        }
    }
}