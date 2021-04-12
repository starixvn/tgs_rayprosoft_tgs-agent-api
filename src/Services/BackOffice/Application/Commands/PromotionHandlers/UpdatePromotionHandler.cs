using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Promotion;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.PromotionHandlers
{
    public class UpdatePromotionHandler : Handler<UpdatePromotionRequest, MessageResponse>
    {
        private readonly IPromotionService _service;

        public UpdatePromotionHandler(IPromotionService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(UpdatePromotionRequest request,
            CancellationToken cancellationToken)
        {
            var promotion = await _service.GetPromotionAsync(q => q.Id == request.Id);

            if (promotion == null)
            {
                return CreateInvalidResponse<MessageResponse>(Promotion.NotFound);
            }

            if (await _service.ExistsAsync(x => request.Id != x.Id && x.Name == request.Name))
            {
                return CreateInvalidResponse<MessageResponse>(Brand.NameExisted);
            }

            await _service.UpdatePromotionAsync(promotion, request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}