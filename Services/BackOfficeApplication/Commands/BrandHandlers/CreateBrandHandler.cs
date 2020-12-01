using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models.Messages;
using TGS.Services.BackOfficeApplication.DataService.Services;
using TGS.Services.BackOfficeApplication.Models.Brand;

namespace TGS.Services.BackOfficeApplication.Commands.BrandHandlers
{
    public class CreateBrandHandler : Handler<CreateBrandRequest, MessageResponse>
    {
        private readonly IBrandService _service;

        public CreateBrandHandler(IBrandService service)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(CreateBrandRequest request,
            CancellationToken cancellationToken)
        {
            await _service.CreateBrandAsync(request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}