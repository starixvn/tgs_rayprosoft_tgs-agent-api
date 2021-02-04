using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Services.PluggableServices.Models;

namespace TGS.Apis.TgsApi.Commons.Handlers
{
    public sealed class GetGameUrlHandler : IRequestHandler<GetGameUrlRequest, GetGameUrlResponse>
    {
        private readonly Func<string, IGameInitializer> _factory;

        public GetGameUrlHandler(Func<string, IGameInitializer> factory)
        {
            _factory = factory;
        }

        Task<GetGameUrlResponse> IRequestHandler<GetGameUrlRequest, GetGameUrlResponse>.Handle(
            GetGameUrlRequest request, CancellationToken cancellationToken) =>
            _factory(request.GameProviderCode).GetGameUrlAsync(request);
    }
}