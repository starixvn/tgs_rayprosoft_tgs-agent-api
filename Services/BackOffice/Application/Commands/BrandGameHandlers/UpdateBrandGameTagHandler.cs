using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.BrandGame;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.BrandGameHandlers
{
    public class UpdateBrandGameTagHandler : Handler<UpdateBrandGameTagRequest, MessageResponse>
    {
        private readonly IBrandGameService _service;

        public UpdateBrandGameTagHandler(IBrandGameService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(UpdateBrandGameTagRequest request,
            CancellationToken cancellationToken)
        {
            var brandGame = await _service.GetBrandGameAsync(q => q.Id == request.Id);

            if (brandGame == null)
            {
                return CreateInvalidResponse<MessageResponse>(BrandGame.NotFound);
            }

            brandGame.Tag = request.Enable
                ? brandGame.Tag.AddFlag(request.GameTag)
                : brandGame.Tag.RemoveFlag(request.GameTag);

            await _service.UpdateBrandGameAsync(brandGame);

            return CreateValidResponse<MessageResponse>();
        }
    }
}