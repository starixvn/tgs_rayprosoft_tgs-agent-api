using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Models.Messages;
using TGS.Services.Operations.Queries;
using TGS.Services.Report.Application.Models.Round;

namespace TGS.Apis.OperatorApi.Handlers.PlayerReport
{
    public sealed class GetPlayerReportHandler : IRequestHandler<GetPlayerRoundReportRequest, PagedResponse<PlayerRoundReportModel>>
    {
        private readonly HttpContext _httpContext;
        private readonly IMediator _mediator;
        private readonly IPlayerQueries _queries;

        public GetPlayerReportHandler(IHttpContextAccessor httpContextAccessor,
            IMediator mediator, IPlayerQueries queries)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _mediator = mediator;
            _queries = queries;
        }

        async Task<PagedResponse<PlayerRoundReportModel>>
            IRequestHandler<GetPlayerRoundReportRequest, PagedResponse<PlayerRoundReportModel>>.Handle(
                GetPlayerRoundReportRequest request, CancellationToken cancellationToken)
        {
            var data = _httpContext.Items.GetPlayerTokenData();

            var brandId = _httpContext.Items.GetBrandId();
            var player = await _queries.GetCachedPlayerAsync(data.PlayerCode);

            var roundModelResponse = await _mediator.Send(
                new GetRoundsRequest
                {
                    BrandId = brandId,
                    Username = player.Username,
                    From = request.From,
                    To = request.To,
                    OrderBy = request.OrderBy,
                    Page = request.Page,
                    ShowAll = request.ShowAll,
                    SortBy = request.SortBy
                }, cancellationToken);

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