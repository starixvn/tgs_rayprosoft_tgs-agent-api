using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.BrandAllAgent;
using TGS.Services.BackOffice.Infrastructure.DataService;
using Brand = TGS.Services.BackOffice.Application.Resources.Brand;

namespace TGS.Services.BackOffice.Application.Commands.BrandAllAgentGameTypeHandlers
{
    public class
        UpdateBrandAllAgentGameTypeGameTypeSwitchHandler : Handler<UpdateBrandAllAgentGameTypeRequest, MessageResponse>
    {
        private readonly IBrandService _brandService;
        private readonly IBrandAllAgentGameTypeService _service;

        public UpdateBrandAllAgentGameTypeGameTypeSwitchHandler(IBrandService brandService,
            IBrandAllAgentGameTypeService service,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
            _brandService = brandService;
        }

        public override async Task<MessageResponse> Handle(UpdateBrandAllAgentGameTypeRequest request,
            CancellationToken cancellationToken)
        {
            var brand = await _brandService.GetBrandAsync(x => x.Id == request.BrandId);

            if (brand == null)
            {
                return CreateInvalidResponse<MessageResponse>(Brand.NotFound);
            }

            var brandAllAgent = await _service.GetBrandAllAgentGameTypeAsync(x => x.BrandId == request.BrandId);

            switch (request.Switch)
            {
                case true when brandAllAgent is null:
                    await _service.CreateBrandAllAgentGameTypeAsync(
                        new BrandAllAgentGameType { BrandId = request.BrandId });
                    break;
                case false when brandAllAgent is not null:
                    await _service.DeleteBrandAllAgentGameTypeAsync(brandAllAgent);
                    break;
            }

            return CreateValidResponse<MessageResponse>();
        }
    }
}