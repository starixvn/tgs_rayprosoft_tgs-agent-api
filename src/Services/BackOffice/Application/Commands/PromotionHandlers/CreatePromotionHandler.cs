using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Promotion;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.PromotionHandlers
{
    public class CreatePromotionHandler : Handler<CreatePromotionRequest, MessageResponse>
    {
        private readonly IPromotionService _service;

        public CreatePromotionHandler(IPromotionService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(CreatePromotionRequest request,
            CancellationToken cancellationToken)
        {
            if (await _service.ExistsAsync(x => x.Name == request.Name))
            {
                return CreateInvalidResponse<MessageResponse>(Promotion.NameExisted);
            }

            await _service.CreatePromotionAsync(request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}