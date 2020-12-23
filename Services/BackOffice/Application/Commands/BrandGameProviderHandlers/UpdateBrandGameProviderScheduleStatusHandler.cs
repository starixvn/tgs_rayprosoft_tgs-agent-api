using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.BrandGameProviderScheduleHandlers
{
    public class
        UpdateBrandGameProviderScheduleStatusHandler : Handler<UpdateGameProviderStatusRequest, MessageResponse>
    {
        private readonly IBrandGameProviderService _service;

        public UpdateBrandGameProviderScheduleStatusHandler(IBrandGameProviderService service,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(UpdateGameProviderStatusRequest request,
            CancellationToken cancellationToken)
        {
            var brandGameProvider = await _service.GetBrandGameProviderAsync(q => q.Id == request.Id);

            if (brandGameProvider == null)
            {
                return CreateInvalidResponse<MessageResponse>(BrandGameProvider.NotFound);
            }

            brandGameProvider.Status = request.Status;

            await _service.UpdateBrandGameProviderAsync(brandGameProvider);

            return CreateValidResponse<MessageResponse>();
        }
    }
}