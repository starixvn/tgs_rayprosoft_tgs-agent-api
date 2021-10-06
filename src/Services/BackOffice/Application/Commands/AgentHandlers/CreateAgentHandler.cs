using System.Linq;
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

                foreach (var condition in request.AgentDividendConditions.OrderBy(x => x.Rank))
                {
                    var parent = parentAgent.AgentDividendConditions.FirstOrDefault(x => x.Rank == condition.Rank);

                    if (parent is null)
                    {
                        return CreateInvalidResponse<MessageResponse>(string.Format(Agent.ParentRankNotFound,
                            condition.Rank));
                    }

                    if (condition.NetProfit < parent.NetProfit)
                    {
                        return CreateInvalidResponse<MessageResponse>(string.Format(Agent.NetProfitMustGreaterThan,
                            condition.Rank, parent.NetProfit));
                    }

                    if (condition.ActiveNumber < parent.ActiveNumber)
                    {
                        return CreateInvalidResponse<MessageResponse>(string.Format(Agent.ActiveNumberMustGreaterThan,
                            condition.Rank, parent.ActiveNumber));
                    }

                    if (condition.DividendPercentage > parent.DividendPercentage)
                    {
                        return CreateInvalidResponse<MessageResponse>(string.Format(
                            Agent.DividendPercentageMustLessThan,
                            condition.Rank, parent.DividendPercentage));
                    }
                }
            }

            foreach (var condition in request.AgentDividendConditions.OrderBy(x => x.Rank))
            {
                var previous = request.AgentDividendConditions.FirstOrDefault(x => x.Rank == condition.Rank - 1);

                if (previous is not null && (condition.NetProfit < previous.NetProfit ||
                    condition.ActiveNumber < previous.ActiveNumber ||
                    condition.DividendPercentage < previous.DividendPercentage))
                {
                    return CreateInvalidResponse<MessageResponse>(string.Format(Agent.RankMustGreaterThanPervious,
                        condition.Rank));
                }
            }

            await _service.CreateAgentAsync(request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}