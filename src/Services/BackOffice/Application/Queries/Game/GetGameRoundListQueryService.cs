using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.Round;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Queries.Game
{
    internal class GetGameRoundListQueryService : IGetService<GetGameRoundListQuery, PagedResponse<RoundModel>>
    {
        private readonly IReportClient _reportClient;

        public GetGameRoundListQueryService(IReportClient reportClient)
        {
            _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));
        }

        public async ValueTask<PagedResponse<RoundModel>> GetAsync(
            GetGameRoundListQuery query,
            CancellationToken cancellationToken = default)
        {
            var reportRequest = new GetRoundsRequest
            {
                BrandId = query.BrandId,
                Username = query.Username,
                PlayerCode = query.PlayerCode,
                RoundId = query.RoundId,
                ExternalRoundId = query.ExternalRoundId,
                GameProviderId = query.GameProviderId,
                Status = query.Status,
                GameType = query.GameType,
                From = query.From,
                To = query.To,
                ClosedFrom = query.ClosedFrom,
                ClosedTo = query.ClosedTo,
                Page = query.Paging.Page,
                Rows = query.Paging.Rows,
                OrderBy = query.Paging.OrderBy,
                SortBy = query.Paging.SortBy,
                ShowAll = query.Paging.ShowAll,
            };

            return await _reportClient
                .GetRoundsAsync(reportRequest)
                .ConfigureAwait(false);
        }
    }
}
