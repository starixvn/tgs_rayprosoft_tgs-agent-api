using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.DataService.Services;
using TGS.Services.BackOffice.Application.Models.Brand;

namespace TGS.Services.BackOffice.Application.Commands.BrandHandlers
{
    public class CreateBrandHandler : Handler<CreateBrandRequest, MessageResponse>
    {
        private readonly IBrandService _service;

        public CreateBrandHandler(IBrandService service, IHttpContextAccessor accessor) : base(accessor)
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