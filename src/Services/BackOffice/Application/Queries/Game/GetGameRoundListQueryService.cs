using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.Round;
using TGS.Core.Models.Messages;
using TGS.ServiceUtility;

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
                ExternalRoundId = query.ExternalRoundId,
                From = query.From,
                GameProviderId = query.GameProviderId,
                GameType = query.GameType,
                OrderBy = query.Paging.OrderBy,
                Page = query.Paging.Page,
                PlayerCode = query.PlayerCode,
                RoundId = query.RoundId,
                Rows = query.Paging.Rows,
                ShowAll = query.Paging.ShowAll,
                SortBy = query.Paging.SortBy,
                Status = query.Status,
                To = query.To,
                Username = query.Username
            };

            return await _reportClient
                .GetRoundsAsync(reportRequest)
                .ConfigureAwait(false);
        }
    }
}
