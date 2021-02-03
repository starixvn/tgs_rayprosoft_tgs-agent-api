using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Services.PluggableServices.Models;

namespace TGS.Apis.TgsApi.Commons.Handlers
{
    public sealed class GetRoundHistoryHandler : IRequestHandler<GetRoundHistoryRequest, GetRoundHistoryResponse>
    {
        private readonly Func<string, IGameHistory> _factory;

        public GetRoundHistoryHandler(Func<string, IGameHistory> factory)
        {
            _factory = factory;
        }

        Task<GetRoundHistoryResponse> IRequestHandler<GetRoundHistoryRequest, GetRoundHistoryResponse>.Handle(
            GetRoundHistoryRequest request, CancellationToken cancellationToken) =>
            _factory(request.GameProviderCode).GetRoundHistoryAsync(request);
    }
}