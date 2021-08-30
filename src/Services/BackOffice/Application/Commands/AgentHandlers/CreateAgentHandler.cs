using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Agent;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.AgentHandlers
{
    public class CreateAgentHandler : Handler<CreateAgentRequest, MessageResponse>
    {
        private readonly IAgentService _service;

        public CreateAgentHandler(IAgentService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(CreateAgentRequest request,
            CancellationToken cancellationToken)
        {
            if (await _service.ExistsAsync(x =>
                x.BrandId == request.BrandId && EF.Functions.ILike(x.Username, $"%{request.Username}%")))
            {
                return CreateInvalidResponse<MessageResponse>(Agent.NameExisted);
            }

            if (request.ParentUsername.IsNotNullOrEmpty())
            {
                var parentAgent = await _service.GetAgentAsync(x => x.BrandId == request.BrandId &&
                    x.Username == request.ParentUsername);

                if (parentAgent == null)
                {
                    return CreateInvalidResponse<MessageResponse>(Agent.ParentAgentNotExists);
                }

                if (parentAgent.DividendPercentage < request.DividendPercentage)
                {
                    return CreateInvalidResponse<MessageResponse>(Agent
                        .DividendPercentageMustLessThanOrEqualToUpperLayer);
                }
            }


            await _service.CreateAgentAsync(request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}