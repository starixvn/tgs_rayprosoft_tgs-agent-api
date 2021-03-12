using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.VipLevel;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.VipLevelHandlers
{
    public class CreateVipLevelHandler : Handler<CreateVipLevelRequest, MessageResponse>
    {
        private readonly IVipLevelService _service;

        public CreateVipLevelHandler(IVipLevelService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(CreateVipLevelRequest request,
            CancellationToken cancellationToken)
        {
            var lastLevel = await _service.GetVipLevelQueries(q => q.Where(v => v.BrandId == request.BrandId))
                .MaxAsync(x => x.Level, cancellationToken);
            var lastVip = await _service.GetVipLevelAsync(x => x.BrandId == request.BrandId && x.Level == lastLevel);

            if (lastVip.LevelUpAccumulateBetAmount >= request.LevelUpAccumulateBetAmount ||
                lastVip.LevelUpAccumulateDeposit >= request.LevelUpAccumulateDeposit)
            {
                return CreateInvalidResponse<MessageResponse>(string.Format(
                    VipLevel.LevelUpAccumulateLessThanPreviousLevel,
                    lastVip.LevelUpAccumulateBetAmount, lastVip.LevelUpAccumulateDeposit));
            }

            request.Level = lastVip.Level + 1;

            await _service.CreateVipLevelAsync(request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}