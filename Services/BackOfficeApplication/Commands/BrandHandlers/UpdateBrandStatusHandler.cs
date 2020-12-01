using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models.Messages;
using TGS.Services.BackOfficeApplication.DataService.Services;
using TGS.Services.BackOfficeApplication.Models.Brand;
using TGS.Services.BackOfficeApplication.Resources;

namespace TGS.Services.BackOfficeApplication.Commands.BrandHandlers
{
    public class UpdateBrandStatusHandler : Handler<UpdateBrandStatusRequest, MessageResponse>
    {
        private readonly IBrandService _service;

        public UpdateBrandStatusHandler(IBrandService service)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(UpdateBrandStatusRequest request,
            CancellationToken cancellationToken)
        {
            var brand = await _service.GetBrandAsync(q => q.Id == request.BrandId);

            if (brand == null)
            {
                return CreateInvalidResponse<MessageResponse>(Brand.NotFound);
            }

            await _service.ChangeStatusAsync(brand, request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}