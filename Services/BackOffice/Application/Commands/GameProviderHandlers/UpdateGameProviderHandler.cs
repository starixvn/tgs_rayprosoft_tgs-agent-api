using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.GameProvider;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.GameProviderHandlers
{
    public class UpdateGameProviderHandler : Handler<UpdateGameProviderRequest, MessageResponse>
    {
        private readonly IGameProviderService _service;

        public UpdateGameProviderHandler(IGameProviderService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(UpdateGameProviderRequest request,
            CancellationToken cancellationToken)
        {
            var gameProvider = await _service.GetGameProviderAsync(x => x.Id == request.Id);

            await _service.UpdateGameProviderAsync(gameProvider, request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}