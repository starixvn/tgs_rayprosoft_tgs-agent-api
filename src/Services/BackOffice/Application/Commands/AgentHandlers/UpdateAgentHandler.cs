using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Agent;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Domain.Enums;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.AgentHandlers
{
    public class UpdateAgentHandler : Handler<UpdateAgentRequest, MessageResponse>
    {
        private readonly IAgentService _service;

        public UpdateAgentHandler(IAgentService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(UpdateAgentRequest request,
            CancellationToken cancellationToken)
        {
            var agent = await _service.GetAgentAsync(q => q.Id == request.Id);

            if (agent == null)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.NotFound);
            }

            if (request.ParentUsername.IsNotNullOrEmpty() &&
                !await _service.ExistsAsync(x =>
                    x.BrandId == request.BrandId && x.Username == request.ParentUsername &&
                    x.LevelType == AgentLevelType.Chief))
            {
                return CreateInvalidResponse<MessageResponse>(Agent.ParentAgentNotExists);
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

            var childAgentMaxDividendPercentage = await _service.GetAgentQueries()
                .Where(x => x.ParentAgentId == agent.Id)
                .MaxAsync(x => (decimal?)x.DividendPercentage, cancellationToken)
                .ConfigureAwait(false) ?? 0M;

            if (request.DividendPercentage < childAgentMaxDividendPercentage)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.DividendPercentageMustLessThanOrEqualToUpperLayer);
            }

            if (request.UpdateType == MemberUpdateType.Contact &&
                request.Email.IsNotNullOrEmpty() &&
                await _service.ExistsAsync(x => x.Id != agent.Id && EF.Functions.ILike(x.Email, $"%{request.Email}%")))
            {
                return CreateInvalidResponse<MessageResponse>(Agent.EmailExisted);
            }

            await _service.UpdateAgentAsync(agent, request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}