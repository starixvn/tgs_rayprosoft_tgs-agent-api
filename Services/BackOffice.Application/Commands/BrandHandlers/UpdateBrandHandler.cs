using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.DataService.Services;
using TGS.Services.BackOffice.Application.Models.Brand;
using TGS.Services.BackOffice.Application.Resources;

namespace TGS.Services.BackOffice.Application.Commands.BrandHandlers
{
    public class UpdateBrandHandler : Handler<UpdateBrandRequest, MessageResponse>
    {
        private readonly IBrandService _service;

        public UpdateBrandHandler(IBrandService service)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(UpdateBrandRequest request,
            CancellationToken cancellationToken)
        {
            var brand = await _service.GetBrandAsync(q => q.Id == request.Id, tracking: true);

            if (brand == null)
            {
                return CreateInvalidResponse<MessageResponse>(Brand.NotFound);
            }

            await _service.UpdateBrandAsync(brand, request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}