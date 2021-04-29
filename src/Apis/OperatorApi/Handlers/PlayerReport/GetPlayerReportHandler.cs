using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Report.Client;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Models.Messages;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.OperatorApi.Handlers.PlayerReport
{
    public sealed class
        GetPlayerReportHandler : IRequestHandler<GetPlayerRoundReportRequest, PagedResponse<PlayerRoundReportModel>>
    {
        private readonly IReportClient _client;
        private readonly HttpContext _httpContext;
        private readonly IMediator _mediator;
        private readonly IPlayerQueries _queries;

        public GetPlayerReportHandler(IHttpContextAccessor httpContextAccessor,
            IMediator mediator, IPlayerQueries queries, IReportClient client)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _mediator = mediator;
            _queries = queries;
            _client = client;
        }

        async Task<PagedResponse<PlayerRoundReportModel>>
            IRequestHandler<GetPlayerRoundReportRequest, PagedResponse<PlayerRoundReportModel>>.Handle(
                GetPlayerRoundReportRequest request, CancellationToken cancellationToken)
        {
            var data = _httpContext.Items.GetPlayerTokenData();

            var brandId = _httpContext.Items.GetBrandId();
            var player = await _queries.GetCachedPlayerAsync(data.PlayerCode);

            var roundModelResponse = await _client.GetRoundsAsync(new()
            {
                BrandId = brandId,
                Username = player.Username,
                From = request.From,
                To = request.To,
                OrderBy = request.OrderBy,
                Page = request.Page,
                ShowAll = request.ShowAll,
                SortBy = request.SortBy
            });

            return new()
            {
                TotalCount = roundModelResponse.TotalCount,
                Items = roundModelResponse.Items.Select(x => new PlayerRoundReportModel
                {
                    Id = x.Id,
                    CreatedOn = x.CreatedOn,
                    GameProviderName = x.GameProviderName,
                    BetAmount = x.BetAmount,
                    ValidBetAmount = x.ValidBetAmount,
                    WinLoss = x.WinLoss
                }).ToArray()
            };
        }
    }
}