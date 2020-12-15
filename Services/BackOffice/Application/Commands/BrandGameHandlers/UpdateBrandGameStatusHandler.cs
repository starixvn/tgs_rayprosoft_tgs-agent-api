using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.BrandGame;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.BrandGameHandlers
{
    public class UpdateBrandGameStatusHandler : Handler<UpdateBrandGameStatusRequest, MessageResponse>
    {
        private readonly IBrandGameService _service;

        public UpdateBrandGameStatusHandler(IBrandGameService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(UpdateBrandGameStatusRequest request,
            CancellationToken cancellationToken)
        {
            var brandGame = await _service.GetBrandGameAsync(q => q.Id == request.Id);

            if (brandGame == null)
            {
                return CreateInvalidResponse<MessageResponse>(BrandGame.NotFound);
            }

            brandGame.Status = request.Status;

            await _service.UpdateBrandGameAsync(brandGame);

            return CreateValidResponse<MessageResponse>();
        }
    }
}