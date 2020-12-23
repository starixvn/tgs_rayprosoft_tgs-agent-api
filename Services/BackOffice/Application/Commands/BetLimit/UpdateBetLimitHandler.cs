using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.BetLimit;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.BetLimit
{
    public class UpdateBetLimitHandler : Handler<UpdateBetLimitRequest, MessageResponse>
    {
        private readonly IBetLimitService _service;

        public UpdateBetLimitHandler(IBetLimitService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(UpdateBetLimitRequest request,
            CancellationToken cancellationToken)
        {
            var betLimit = await _service.GetBetLimitAsync(q => q.Id == request.Id);

            if (betLimit == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.BetLimit.NotFound);
            }

            await _service.UpdateBetLimitAsync(betLimit, request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}